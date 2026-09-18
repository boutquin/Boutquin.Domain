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

#pragma warning disable CA2007 // Consider calling ConfigureAwait — suppressed per xUnit1030 guidance

namespace Boutquin.UnitTests.AspNetCore;

using System.Text.Json;
using Boutquin.AspNetCore;
using Boutquin.Domain.Exceptions;
using Boutquin.Validation.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/// <summary>
/// Tests for the CustomExceptionHandlerMiddleware to ensure all domain exceptions
/// are correctly mapped to their corresponding HTTP status codes.
/// </summary>
public sealed class CustomExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenBadRequestException_Returns400()
    {
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new BadRequestException("Invalid input"));

        statusCode.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Title.Should().Be("Bad Request");
        problemDetails.Detail.Should().Be("Invalid input");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedException_Returns401()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new UnauthorizedException("Not authenticated"));

        statusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WhenForbiddenException_Returns403()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new ForbiddenException("Access denied"));

        statusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_Returns404()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new NotFoundException("Resource not found"));

        statusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task InvokeAsync_WhenConflictException_Returns409()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new ConflictException("Conflict detected"));

        statusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnsupportedMediaTypeException_Returns415()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new UnsupportedMediaTypeException("Unsupported type"));

        statusCode.Should().Be(StatusCodes.Status415UnsupportedMediaType);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnprocessableEntityException_Returns422()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new UnprocessableEntityException("Cannot process"));

        statusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task InvokeAsync_WhenTooManyRequestsException_Returns429()
    {
        var (statusCode, _) = await InvokeMiddlewareWithException(
            new TooManyRequestsException("Rate limited"));

        statusCode.Should().Be(StatusCodes.Status429TooManyRequests);
    }

    [Theory]
    [MemberData(nameof(MessagelessClientExceptionCases))]
    public async Task InvokeAsync_MessagelessClientException_DetailDoesNotLeakInternalTypeName(
        Exception exception)
    {
        // F-L5-01 / SEC-4XX-TYPENAME-001 (information disclosure): a 4xx DomainException thrown
        // via its message-less constructor has no caller-supplied Message, so Exception.Message
        // falls back to the .NET framework default — "Exception of type
        // 'Boutquin.Domain.Exceptions.NotFoundException' was thrown." — which embeds the internal,
        // fully-qualified type name. The middleware forwards 4xx Message verbatim (intended, D-01),
        // so without a safe default that internal detail reaches the client. The Detail must never
        // carry the namespace, the "was thrown" framework boilerplate, or the exception's type name.
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(exception);

        statusCode.Should().BeLessThan(StatusCodes.Status500InternalServerError);
        problemDetails.Detail.Should().NotBeNullOrWhiteSpace();
        problemDetails.Detail.Should().NotContain("Boutquin.Domain.Exceptions");
        problemDetails.Detail.Should().NotContain("was thrown");
        problemDetails.Detail.Should().NotContain(exception.GetType().Name);
    }

    public static TheoryData<Exception> MessagelessClientExceptionCases() => new()
    {
        new BadRequestException(),
        new UnauthorizedException(),
        new ForbiddenException(),
        new NotFoundException(),
        new ConflictException(),
        new UnsupportedMediaTypeException(),
        new UnprocessableEntityException(),
        new TooManyRequestsException(),
    };

    [Fact]
    public async Task InvokeAsync_MessagelessNotFoundException_DetailIsSafeGenericPhrase()
    {
        // F-L5-01: positive pin — a message-less 4xx exception surfaces the safe, client-facing
        // default phrase (ExceptionMessages.NotFound), not the leaked framework default.
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(new NotFoundException());

        statusCode.Should().Be(StatusCodes.Status404NotFound);
        problemDetails.Title.Should().Be("Not Found");
        problemDetails.Detail.Should().Be(ExceptionMessages.NotFound);
    }

    [Fact]
    public async Task InvokeAsync_NotFoundExceptionWithCallerMessage_ForwardsMessageVerbatim()
    {
        // F-L5-01 guard: the fix must NOT suppress a genuine caller-supplied 4xx message. When the
        // caller passes a message (the intended, client-facing D-01 path), it is forwarded as-is.
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new NotFoundException("User 42 not found"));

        statusCode.Should().Be(StatusCodes.Status404NotFound);
        problemDetails.Detail.Should().Be("User 42 not found");
    }

    [Fact]
    public async Task InvokeAsync_WhenInternalServerErrorException_Returns500WithGenericDetail()
    {
        // Security: InternalServerErrorException represents a server-side fault. Echoing its
        // Message to the client would leak internal details (connection strings, SQL, paths).
        // The detail is deliberately generic; only the (safe) title is surfaced.
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new InternalServerErrorException("server=db;pwd=topsecret connection failed"));

        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("Internal Server Error");
        problemDetails.Detail.Should().Be("An unexpected error occurred.");
        problemDetails.Detail.Should().NotContain("topsecret");
    }

    [Fact]
    public async Task InvokeAsync_WhenServiceUnavailableException_Returns503()
    {
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new ServiceUnavailableException("Service down"));

        statusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
        problemDetails.Title.Should().Be("Service Unavailable");
    }

    [Fact]
    public async Task InvokeAsync_WhenServerSideDomainException_SuppressesMessageDetail()
    {
        // D-01: message suppression must key on the >= 500 status CLASS, not the single
        // InternalServerErrorException type. ServiceUnavailableException (503, : DomainException)
        // carries an internal detail that must never leak; only the generic detail may surface,
        // while the (safe) status and title are still forwarded.
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new ServiceUnavailableException("internal detail"));

        statusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
        problemDetails.Title.Should().Be("Service Unavailable");
        problemDetails.Detail.Should().Be("An unexpected error occurred.");
        problemDetails.Detail.Should().NotContain("internal detail");
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_NullPropertyName_Returns400UnderEmptyKey()
    {
        // D-02: a model-level FluentValidation failure carries PropertyName == null. The handler's
        // GroupBy(...).ToDictionary(...) must coalesce the null key to "" instead of throwing
        // ArgumentNullException (which would let the exception escape the middleware).
        var exception = new ValidationException(new[]
        {
            new ValidationFailure(null, "A model-level error occurred."),
        });

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw exception);

        var act = async () => await middleware.InvokeAsync(context);
        await act.Should().NotThrowAsync();

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = await JsonDocument.ParseAsync(context.Response.Body);
        var errors = doc.RootElement.GetProperty("errors");
        errors.TryGetProperty("", out var emptyKeyErrors).Should().BeTrue();
        emptyKeyErrors[0].GetString().Should().Be("A model-level error occurred.");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnknownException_Returns500WithGenericMessage()
    {
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new InvalidOperationException("Something went wrong"));

        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("Internal Server Error");
        problemDetails.Detail.Should().Be("An unexpected error occurred.");
    }

    [Fact]
    public async Task InvokeAsync_WhenMappedException_EmitsProblemJsonContentType()
    {
        // RFC 7807 mandates the "application/problem+json" media type. Regression guard: the
        // parameterless WriteAsJsonAsync overload hard-codes "application/json" and would silently
        // overwrite the content type set on Response.ContentType beforehand.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(
            _ => throw new BadRequestException("Invalid input"));

        await middleware.InvokeAsync(context);

        context.Response.ContentType.Should().NotBeNull();
        context.Response.ContentType.Should().Contain("application/problem+json");
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_PassesThrough()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Theory]
    [MemberData(nameof(DomainExceptionTitleCases))]
    public async Task InvokeAsync_DomainException_MapsStatusAndTitle(Exception exception, int expectedStatus, string expectedTitle)
    {
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(exception);

        statusCode.Should().Be(expectedStatus);
        problemDetails.Title.Should().Be(expectedTitle);
        problemDetails.Status.Should().Be(expectedStatus);
    }

    public static TheoryData<Exception, int, string> DomainExceptionTitleCases() => new()
    {
        { new BadRequestException("x"), StatusCodes.Status400BadRequest, "Bad Request" },
        { new UnauthorizedException("x"), StatusCodes.Status401Unauthorized, "Unauthorized" },
        { new ForbiddenException("x"), StatusCodes.Status403Forbidden, "Forbidden" },
        { new NotFoundException("x"), StatusCodes.Status404NotFound, "Not Found" },
        { new ConflictException("x"), StatusCodes.Status409Conflict, "Conflict" },
        { new UnsupportedMediaTypeException("x"), StatusCodes.Status415UnsupportedMediaType, "Unsupported Media Type" },
        { new UnprocessableEntityException("x"), StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity" },
        { new TooManyRequestsException("x"), StatusCodes.Status429TooManyRequests, "Too Many Requests" },
        { new ServiceUnavailableException("x"), StatusCodes.Status503ServiceUnavailable, "Service Unavailable" },
    };

    [Fact]
    public async Task InvokeAsync_SetsProblemDetailsInstanceToRequestPath()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/resource/42";
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw new NotFoundException("missing"));

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        problem!.Instance.Should().Be("/api/resource/42");
        problem.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task InvokeAsync_EmitsProblemDetailsTypeAsAboutBlank()
    {
        // P-04 / AC-3.4: RFC 7807's `type` defaults to "about:blank" when omitted; this middleware
        // sets it explicitly (see the production comment at the ProblemDetails construction site) so
        // every emitted response carries an unambiguous, pinnable `type` rather than relying on each
        // client to apply the RFC default itself.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(
            _ => throw new BadRequestException("Invalid input"));

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("type").GetString().Should().Be("about:blank");
    }

    [Fact]
    public void Constructor_WithNullNext_ThrowsArgumentNullException()
    {
        var act = () => new CustomExceptionHandlerMiddleware(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("next");
    }

    [Fact]
    public async Task InvokeAsync_UnmappedException_WithLogger_LogsErrorOnceWithOriginalException()
    {
        // D-03 / DE-5: an unmapped exception reaches the default arm; with a logger injected, exactly
        // one LogError is recorded carrying the original exception (this outermost catch is the only
        // place it can be logged — it does not rethrow).
        var logger = new CapturingLogger<CustomExceptionHandlerMiddleware>();
        var original = new InvalidOperationException("boom");
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw original, logger);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        logger.Entries.Should().ContainSingle();
        logger.Entries[0].Level.Should().Be(LogLevel.Error);
        logger.Entries[0].Exception.Should().BeSameAs(original);
    }

    [Fact]
    public async Task InvokeAsync_UnmappedException_WithoutLogger_DoesNotThrow()
    {
        // With no logger (null), behavior is unchanged: the null-conditional log call is skipped and
        // the generic 500 ProblemDetails is still written.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw new InvalidOperationException("boom"));

        var act = async () => await middleware.InvokeAsync(context);

        await act.Should().NotThrowAsync();
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_WhenDownstreamBuffersBytesThenThrows_ResponseBodyIsOnlyProblemJson()
    {
        // G-01 / AC-2.1: a downstream component writes buffered bytes to the not-yet-started response,
        // then throws. The middleware must Response.Clear() the buffered bytes so the client receives
        // ONLY the ProblemDetails JSON, not a corrupt mixed body — and clearing must not drop the status.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(async ctx =>
        {
            await ctx.Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes("PARTIAL_GARBAGE_BYTES"));
            throw new BadRequestException("boom");
        });

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        body.Should().NotContain("PARTIAL_GARBAGE_BYTES");
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status400BadRequest);
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenRequestAbortedAndOperationCanceled_Rethrows()
    {
        // G-02 / AC-2.2: when the client aborts (RequestAborted cancelled) and the pipeline throws
        // OperationCanceledException, the middleware must rethrow rather than attempt a 500 write to a
        // dead connection.
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var context = new DefaultHttpContext { RequestAborted = cts.Token };
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw new OperationCanceledException());

        var act = async () => await middleware.InvokeAsync(context);

        await act.Should().ThrowAsync<OperationCanceledException>();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WhenOperationCanceledButNotAborted_Returns500()
    {
        // A non-abort OperationCanceledException (request NOT aborted) is a genuine fault and still
        // produces the generic 500 ProblemDetails — the rethrow filter must not swallow it.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw new OperationCanceledException());

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_EmptyErrors_Returns400WithEmptyErrorsDict()
    {
        // AC-4.1: a ValidationException carrying zero failures (an edge case distinct from the
        // null-PropertyName case above) must still produce a 400 with an `errors` object present — an
        // EMPTY dictionary, not a missing property or a crash from GroupBy/ToDictionary over an empty
        // sequence.
        var exception = new ValidationException(Array.Empty<ValidationFailure>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw exception);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = await JsonDocument.ParseAsync(context.Response.Body);
        var errors = doc.RootElement.GetProperty("errors");
        errors.ValueKind.Should().Be(JsonValueKind.Object);
        errors.EnumerateObject().Should().BeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_EmitsProblemJsonContentType()
    {
        // AC-4.1: the RFC 7807 content type must be asserted on the ValidationException branch
        // specifically, not just the DomainException branch that
        // InvokeAsync_WhenMappedException_EmitsProblemJsonContentType already covers.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(
            _ => throw new ValidationException(new[] { new ValidationFailure("Field", "bad") }));

        await middleware.InvokeAsync(context);

        context.Response.ContentType.Should().NotBeNull();
        context.Response.ContentType.Should().Contain("application/problem+json");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnmappedException_EmitsProblemJsonContentType()
    {
        // AC-4.1: the RFC 7807 content type must be asserted on the default (unmapped-exception, 500)
        // branch too — a distinct code path from both the ValidationException and DomainException arms.
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(
            _ => throw new InvalidOperationException("boom"));

        await middleware.InvokeAsync(context);

        context.Response.ContentType.Should().NotBeNull();
        context.Response.ContentType.Should().Contain("application/problem+json");
    }

    [Theory]
    [MemberData(nameof(NonDomainExceptionCases))]
    public async Task InvokeAsync_WhenNonDomainDomainLikeException_Returns500WithGenericDetail(Exception exception)
    {
        // AC-4.1: CurrencyMismatchException, EmptyOrNullCollectionException, and
        // InsufficientDataException are all domain-meaningful failures but none derive from
        // DomainException — they must fall through to the default 500-generic-detail branch like any
        // other unmapped exception, never leak their (potentially sensitive) Message.
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(exception);

        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("Internal Server Error");
        problemDetails.Detail.Should().Be("An unexpected error occurred.");
    }

    public static TheoryData<Exception> NonDomainExceptionCases() => new()
    {
        new CurrencyMismatchException(Boutquin.Domain.Enumerations.Currency.USD, Boutquin.Domain.Enumerations.Currency.EUR),
        new EmptyOrNullCollectionException(),
        new InsufficientDataException("not enough data"),
    };

    // Minimal ILogger that records the level and exception of each Log call, for asserting on the
    // middleware's error logging without a mocking framework.
    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, Exception? Exception)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, exception));
    }

    private static async Task<(int StatusCode, ProblemDetails ProblemDetails)> InvokeMiddlewareWithException(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw exception);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return (context.Response.StatusCode, problemDetails!);
    }
}
