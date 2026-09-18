// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License").
//   You may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace Boutquin.AspNetCore;

using System.Runtime.ExceptionServices;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Validation.Exceptions;

/// <summary>
/// A custom exception handler middleware that catches exceptions, maps them to the appropriate
/// HTTP status codes, and creates RFC 7807 ProblemDetails responses.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why middleware instead of exception filters:</b> ASP.NET Core offers two main interception
/// points for exceptions — MVC exception filters (<c>IExceptionFilter</c>) and middleware.
/// Filters only run for exceptions thrown within the MVC pipeline (controller actions, model binding,
/// action filters). Middleware, by contrast, wraps the <em>entire</em> request pipeline, catching
/// exceptions from any layer: routing, authentication, other middleware, and the MVC pipeline itself.
/// Because domain exceptions can originate from middleware-registered services or non-MVC endpoints
/// (e.g., minimal APIs), middleware is the only interception point that guarantees complete coverage.
/// </para>
/// <para>
/// <b>Why ProblemDetails (RFC 7807):</b> Returning raw status codes or ad-hoc JSON error shapes
/// forces every API client to implement custom error parsing per endpoint. RFC 7807 defines a
/// standard error envelope (<c>type</c>, <c>title</c>, <c>status</c>, <c>detail</c>, <c>instance</c>)
/// that any HTTP client library can parse uniformly. The <c>application/problem+json</c> content type
/// signals to clients that the response body conforms to this standard, enabling generic error handling
/// logic. ASP.NET Core's <see cref="ProblemDetails"/> class maps directly to the RFC 7807 schema.
/// </para>
/// <para>
/// <b>Why three branches in the switch (ValidationException, DomainException, default):</b>
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       <b>ValidationException first:</b> <see cref="ValidationException"/> carries structured
///       validation errors (property name → error messages). These must be surfaced as a grouped
///       dictionary in the <c>errors</c> extension property so that clients can map errors back to
///       specific form fields. Validation failures are always HTTP 400 (Bad Request), regardless of
///       what the exception message says — they represent malformed input, not a server-side fault.
///       <see cref="ValidationException"/> lives in the separate <c>Boutquin.Validation</c> assembly
///       and derives from <see cref="Exception"/>, not <see cref="DomainException"/>; checking it
///       first ensures the structured-errors branch fires for all validation failures regardless of
///       the unrelated <see cref="DomainException"/> hierarchy.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>DomainException second:</b> <see cref="DomainException"/> subclasses carry their own
///       <c>StatusCode</c> and <c>Title</c> (e.g., <see cref="NotFoundException"/> → 404, "Not Found").
///       This self-describing pattern means the middleware doesn't need a growing mapping table —
///       each exception type knows its HTTP semantics. The middleware forwards those values into the
///       ProblemDetails response, but suppresses the <c>Message</c> for any server-side fault whose
///       <c>StatusCode</c> is &gt;= 500 (e.g., <see cref="InternalServerErrorException"/> → 500,
///       <see cref="ServiceUnavailableException"/> → 503): a 5xx message may carry connection strings,
///       SQL, or stack traces and is replaced with the same generic detail as the default branch.
///       Suppression keys on the &gt;= 500 status <em>class</em>, not on any single exception type, so
///       every present and future 5xx domain type is covered uniformly. Client-facing 4xx messages are
///       forwarded as-is.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Default last:</b> Any exception that isn't a known domain or validation type is treated
///       as an Internal Server Error (500). The detail message is intentionally generic ("An unexpected
///       error occurred.") to avoid leaking stack traces, SQL queries, or other internal details to
///       API consumers. Because this is the outermost catch, the original exception is logged
///       <em>here</em> — via the optionally-injected <see cref="ILogger{TCategoryName}"/> — rather than
///       "before this point"; <see cref="InternalServerErrorException"/> is logged the same way in the
///       DomainException arm.
///     </description>
///   </item>
/// </list>
/// </remarks>
public sealed class CustomExceptionHandlerMiddleware
{
    // Cached so the error path does not allocate (or resolve) serializer options per request.
    // Web defaults emit camelCase member names, matching RFC 7807's lower-case field convention.
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlerMiddleware>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why the logger is optional and nullable:</b> When registered via
    /// <c>UseMiddleware&lt;CustomExceptionHandlerMiddleware&gt;()</c>, ASP.NET Core resolves the
    /// <see cref="ILogger{TCategoryName}"/> from the application's DI container automatically, so
    /// server-side faults are logged in production. Making the parameter an optional nullable keeps
    /// direct construction — unit tests and bare (non-DI) pipelines — compiling and working without a
    /// logger; when it is <see langword="null"/>, logging is simply skipped and nothing throws.
    /// </para>
    /// </remarks>
    /// <param name="next">The delegate representing the next middleware in the request pipeline.</param>
    /// <param name="logger">
    /// An optional logger used to record unmapped and internal-server faults. Defaults to
    /// <see langword="null"/>, in which case no logging is performed.
    /// </param>
    public CustomExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<CustomExceptionHandlerMiddleware>? logger = null)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to handle the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why a blanket try/catch here:</b> The middleware pattern requires catching at the outermost
    /// boundary so that unhandled exceptions never reach the Kestrel server, which would respond with
    /// a bare 500 and no body. By catching here, we guarantee every error response is a well-formed
    /// ProblemDetails JSON payload, regardless of where in the pipeline the exception originated.
    /// </para>
    /// <para>
    /// <b>Why ConfigureAwait(false):</b> This middleware has no need to resume on the original
    /// synchronization context after awaiting the next delegate. Using <c>ConfigureAwait(false)</c>
    /// avoids unnecessary context-switching overhead in ASP.NET Core, where there is no
    /// <c>SynchronizationContext</c> by default — but the explicit call documents the intent and
    /// protects against future hosting environments that might install one.
    /// </para>
    /// </remarks>
    /// <param name="context">The <see cref="HttpContext"/> for the current request and response.</param>
    /// <returns>A <see cref="Task"/> that represents the middleware's execution.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client aborted the request (its cancellation token fired). There is no client left to
            // receive a ProblemDetails body, and writing one would be wasted work against a dead
            // connection — so rethrow and let the host tear the request down. A non-abort
            // OperationCanceledException (RequestAborted not signalled) is a genuine fault and falls
            // through to the general handler below, which maps it to a 500.
            throw;
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handles exceptions by mapping them to the appropriate HTTP status codes and creating
    /// ProblemDetails responses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why an instance method:</b> The handler reads the injected <see cref="ILogger{TCategoryName}"/>
    /// (<c>_logger</c>) to record server-side faults, so it depends on instance state and cannot be
    /// static.
    /// </para>
    /// </remarks>
    /// <param name="context">The <see cref="HttpContext"/> for the current request and response.</param>
    /// <param name="exception">The <see cref="Exception"/> that was thrown.</param>
    /// <returns>A <see cref="Task"/> that represents the handling of the exception.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // If the response has already begun streaming, the status line and headers are committed and
        // cannot be rewritten — attempting to set StatusCode or write a body here would throw
        // "headers already sent", masking the original exception and corrupting the response. Re-throw
        // (preserving the original stack trace) so the server tears the connection down cleanly.
        if (context.Response.HasStarted)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        // Discard any bytes a downstream component already buffered into the (not-yet-started) response
        // before it threw. Without this, those partial bytes would prefix the ProblemDetails payload and
        // corrupt the body into non-parseable JSON. Clear() resets the status line to 200 and truncates
        // the body, so it MUST run before the status code is (re)assigned below — the order is:
        // HasStarted guard -> Clear() -> switch -> set StatusCode -> write.
        context.Response.Clear();

        // Default to 500 — the switch only overrides this for recognized exception types.
        // This ensures unknown exceptions always produce a server-error status code.
        var statusCode = StatusCodes.Status500InternalServerError;

        // P-04: RFC 7807 treats "about:blank" as the implicit default for `type` when the field is
        // omitted, but this middleware sets it explicitly rather than relying on clients to apply that
        // default themselves — every response then carries an unambiguous, testable `type`, and a
        // future exception category that wants a dereferenceable problem-type URI can override it per
        // branch without changing this baseline.
        var problemDetails = new ProblemDetails { Type = "about:blank" };

        switch (exception)
        {
            // ValidationException is checked first because it carries structured error data
            // that must be surfaced differently from a generic DomainException. The errors
            // are grouped by property name so that API clients can map each error to the
            // corresponding form field or JSON property that caused the validation failure.
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = "One or more validation errors occurred.";
                problemDetails.Extensions["errors"] = validationException.Errors
                    // A model-level FluentValidation failure carries PropertyName == null. Dictionary
                    // rejects a null key, so the null property name is coalesced to "" — the failure is
                    // surfaced under the empty key rather than crashing the handler itself.
                    .GroupBy(x => x.PropertyName ?? string.Empty)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(x => x.ErrorMessage).ToArray());
                break;

            // DomainException subclasses are self-describing: each carries its own StatusCode
            // and Title. This avoids a growing if/else chain as new exception types are added —
            // the middleware just forwards whatever the exception declares about itself.
            case DomainException domainException:
                statusCode = domainException.StatusCode;
                problemDetails.Title = domainException.Title;

                // Message suppression keys on the >= 500 status CLASS, not a single exception type.
                // A server-side (5xx) fault — InternalServerErrorException (500),
                // ServiceUnavailableException (503), or any future 5xx domain type — may carry
                // connection strings, SQL, or stack traces that must never reach an API consumer, so
                // its Message is replaced with the same generic detail as the default branch. The
                // (safe) status and title are still forwarded. Client-facing 4xx faults keep their
                // Message, which is intended to be read by the caller.
                problemDetails.Detail = domainException.StatusCode >= StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : domainException.Message;

                // Log server-generated internal faults. Only InternalServerErrorException is logged
                // here: it represents a fault this application raised and wants recorded. A 5xx domain
                // *signal* such as ServiceUnavailableException is an expected, self-described condition
                // (its message is already suppressed above), not an internal error to log.
                if (domainException is InternalServerErrorException)
                {
                    _logger?.LogError(exception, "An internal server error was handled by the exception middleware.");
                }

                break;

            // The default branch uses a generic message intentionally — exposing the real
            // exception message (which may contain SQL, stack traces, or internal paths) to
            // API consumers would be an information disclosure vulnerability. This is the outermost
            // catch, so an unmapped exception is logged HERE (it is not, and cannot be, logged
            // "before this point") — provided a logger was injected.
            default:
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
                _logger?.LogError(exception, "An unhandled exception was caught by the exception middleware.");
                break;
        }

        // Instance is set to the request path so that the client can correlate error responses
        // with the specific endpoint that was called — useful in logs and error dashboards.
        problemDetails.Status = statusCode;
        problemDetails.Instance = context.Request.Path;

        // The content type "application/problem+json" is mandated by RFC 7807 — it signals to
        // HTTP clients that the body conforms to the ProblemDetails schema, enabling generic
        // error-handling middleware on the client side. The content type MUST be passed to
        // WriteAsJsonAsync explicitly: the parameterless overload hard-codes "application/json"
        // and would otherwise overwrite any value assigned to Response.ContentType beforehand.
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            s_jsonOptions,
            contentType: "application/problem+json").ConfigureAwait(false);
    }
}
