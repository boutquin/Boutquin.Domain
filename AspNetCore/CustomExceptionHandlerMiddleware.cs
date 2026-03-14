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

using System.Net;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
///       This branch is checked first because <see cref="ValidationException"/> is more specific
///       than <see cref="DomainException"/>; if ValidationException ever inherits from DomainException
///       in the future, the ordering ensures the specialized handler still takes precedence.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>DomainException second:</b> <see cref="DomainException"/> subclasses carry their own
///       <c>StatusCode</c> and <c>Title</c> (e.g., <see cref="NotFoundException"/> → 404, "Not Found").
///       This self-describing pattern means the middleware doesn't need a growing mapping table —
///       each exception type knows its HTTP semantics. The middleware simply forwards those values
///       into the ProblemDetails response.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Default last:</b> Any exception that isn't a known domain or validation type is treated
///       as an Internal Server Error (500). The detail message is intentionally generic ("An unexpected
///       error occurred.") to avoid leaking stack traces, SQL queries, or other internal details to
///       API consumers. In production, the original exception should be logged separately via
///       ILogger before reaching this point.
///     </description>
///   </item>
/// </list>
/// </remarks>
public sealed class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate representing the next middleware in the request pipeline.</param>
    public CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
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
    /// <b>Why static:</b> This method does not access any instance state — it only uses its
    /// parameters. Making it static communicates this clearly and prevents accidental coupling
    /// to instance fields in the future.
    /// </para>
    /// </remarks>
    /// <param name="context">The <see cref="HttpContext"/> for the current request and response.</param>
    /// <param name="exception">The <see cref="Exception"/> that was thrown.</param>
    /// <returns>A <see cref="Task"/> that represents the handling of the exception.</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Default to 500 — the switch only overrides this for recognized exception types.
        // This ensures unknown exceptions always produce a server-error status code.
        var statusCode = StatusCodes.Status500InternalServerError;
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            // ValidationException is checked first because it carries structured error data
            // that must be surfaced differently from a generic DomainException. The errors
            // are grouped by property name so that API clients can map each error to the
            // corresponding form field or JSON property that caused the validation failure.
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "One or more validation errors occurred.";
                problemDetails.Detail = validationException.Message;
                problemDetails.Extensions["errors"] = validationException.Errors
                    .GroupBy(x => x.PropertyName)
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
                problemDetails.Detail = domainException.Message;
                break;

            // The default branch uses a generic message intentionally — exposing the real
            // exception message (which may contain SQL, stack traces, or internal paths) to
            // API consumers would be an information disclosure vulnerability.
            default:
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
                break;
        }

        // Instance is set to the request path so that the client can correlate error responses
        // with the specific endpoint that was called — useful in logs and error dashboards.
        problemDetails.Status = statusCode;
        problemDetails.Instance = context.Request.Path;

        // The content type "application/problem+json" is mandated by RFC 7807 — it signals to
        // HTTP clients that the body conforms to the ProblemDetails schema, enabling generic
        // error-handling middleware on the client side.
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails).ConfigureAwait(false);
    }
}
