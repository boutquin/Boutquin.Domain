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

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for the <see cref="DomainException"/> hierarchy.
/// </summary>
public sealed class DomainExceptionTests
{
    // ── BadRequestException (400) ──────────────────────────────────────

    [Fact]
    public void BadRequestException_HasStatusCode400AndCorrectTitle()
    {
        var exception = new BadRequestException();

        exception.StatusCode.Should().Be(400);
        exception.Title.Should().Be("Bad Request");
    }

    [Fact]
    public void BadRequestException_WithMessage_StoresMessage()
    {
        const string message = "Invalid input data.";

        var exception = new BadRequestException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(400);
        exception.Title.Should().Be("Bad Request");
    }

    [Fact]
    public void BadRequestException_WithInnerException_PreservesInner()
    {
        const string message = "Validation failed.";
        var inner = new InvalidOperationException("Inner error.");

        var exception = new BadRequestException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(400);
    }

    // ── UnauthorizedException (401) ────────────────────────────────────

    [Fact]
    public void UnauthorizedException_HasStatusCode401AndCorrectTitle()
    {
        var exception = new UnauthorizedException();

        exception.StatusCode.Should().Be(401);
        exception.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public void UnauthorizedException_WithMessage_StoresMessage()
    {
        const string message = "Not authenticated.";

        var exception = new UnauthorizedException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(401);
    }

    [Fact]
    public void UnauthorizedException_WithInnerException_PreservesInner()
    {
        const string message = "Auth failed.";
        var inner = new InvalidOperationException("Token expired.");

        var exception = new UnauthorizedException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(401);
    }

    // ── ForbiddenException (403) ───────────────────────────────────────

    [Fact]
    public void ForbiddenException_HasStatusCode403AndCorrectTitle()
    {
        var exception = new ForbiddenException();

        exception.StatusCode.Should().Be(403);
        exception.Title.Should().Be("Forbidden");
    }

    [Fact]
    public void ForbiddenException_WithMessage_StoresMessage()
    {
        const string message = "Access denied.";

        var exception = new ForbiddenException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(403);
    }

    [Fact]
    public void ForbiddenException_WithInnerException_PreservesInner()
    {
        const string message = "Insufficient permissions.";
        var inner = new InvalidOperationException("Role mismatch.");

        var exception = new ForbiddenException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(403);
    }

    // ── NotFoundException (404) ────────────────────────────────────────

    [Fact]
    public void NotFoundException_HasStatusCode404AndCorrectTitle()
    {
        var exception = new NotFoundException();

        exception.StatusCode.Should().Be(404);
        exception.Title.Should().Be("Not Found");
    }

    [Fact]
    public void NotFoundException_WithMessage_StoresMessage()
    {
        const string message = "Resource not found.";

        var exception = new NotFoundException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(404);
    }

    [Fact]
    public void NotFoundException_WithInnerException_PreservesInner()
    {
        const string message = "Entity missing.";
        var inner = new InvalidOperationException("DB lookup failed.");

        var exception = new NotFoundException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(404);
    }

    // ── ConflictException (409) ────────────────────────────────────────

    [Fact]
    public void ConflictException_HasStatusCode409AndCorrectTitle()
    {
        var exception = new ConflictException();

        exception.StatusCode.Should().Be(409);
        exception.Title.Should().Be("Conflict");
    }

    [Fact]
    public void ConflictException_WithMessage_StoresMessage()
    {
        const string message = "Duplicate entry.";

        var exception = new ConflictException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(409);
    }

    [Fact]
    public void ConflictException_WithInnerException_PreservesInner()
    {
        const string message = "Concurrency conflict.";
        var inner = new InvalidOperationException("Version mismatch.");

        var exception = new ConflictException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(409);
    }

    // ── UnsupportedMediaTypeException (415) ────────────────────────────

    [Fact]
    public void UnsupportedMediaTypeException_HasStatusCode415AndCorrectTitle()
    {
        var exception = new UnsupportedMediaTypeException();

        exception.StatusCode.Should().Be(415);
        exception.Title.Should().Be("Unsupported Media Type");
    }

    [Fact]
    public void UnsupportedMediaTypeException_WithMessage_StoresMessage()
    {
        const string message = "Content type not supported.";

        var exception = new UnsupportedMediaTypeException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(415);
    }

    [Fact]
    public void UnsupportedMediaTypeException_WithInnerException_PreservesInner()
    {
        const string message = "Invalid media type.";
        var inner = new InvalidOperationException("Parser error.");

        var exception = new UnsupportedMediaTypeException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(415);
    }

    // ── UnprocessableEntityException (422) ─────────────────────────────

    [Fact]
    public void UnprocessableEntityException_HasStatusCode422AndCorrectTitle()
    {
        var exception = new UnprocessableEntityException();

        exception.StatusCode.Should().Be(422);
        exception.Title.Should().Be("Unprocessable Entity");
    }

    [Fact]
    public void UnprocessableEntityException_WithMessage_StoresMessage()
    {
        const string message = "Semantic error.";

        var exception = new UnprocessableEntityException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(422);
    }

    [Fact]
    public void UnprocessableEntityException_WithInnerException_PreservesInner()
    {
        const string message = "Validation rules violated.";
        var inner = new InvalidOperationException("Field constraint.");

        var exception = new UnprocessableEntityException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(422);
    }

    // ── TooManyRequestsException (429) ─────────────────────────────────

    [Fact]
    public void TooManyRequestsException_HasStatusCode429AndCorrectTitle()
    {
        var exception = new TooManyRequestsException();

        exception.StatusCode.Should().Be(429);
        exception.Title.Should().Be("Too Many Requests");
    }

    [Fact]
    public void TooManyRequestsException_WithMessage_StoresMessage()
    {
        const string message = "Rate limit exceeded.";

        var exception = new TooManyRequestsException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(429);
    }

    [Fact]
    public void TooManyRequestsException_WithInnerException_PreservesInner()
    {
        const string message = "Throttled.";
        var inner = new InvalidOperationException("Quota exceeded.");

        var exception = new TooManyRequestsException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(429);
    }

    // ── InternalServerErrorException (500) ─────────────────────────────

    [Fact]
    public void InternalServerErrorException_HasStatusCode500()
    {
        var exception = new InternalServerErrorException();

        exception.StatusCode.Should().Be(500);
        exception.Title.Should().Be("Internal Server Error");
    }

    [Fact]
    public void InternalServerErrorException_WithMessage_StoresMessage()
    {
        const string message = "Unexpected failure.";

        var exception = new InternalServerErrorException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(500);
    }

    [Fact]
    public void InternalServerErrorException_WithInnerException_PreservesInner()
    {
        const string message = "Something went wrong.";
        var inner = new InvalidOperationException("Inner error.");

        var exception = new InternalServerErrorException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(500);
    }

    // ── ServiceUnavailableException (503) ──────────────────────────────

    [Fact]
    public void ServiceUnavailableException_HasStatusCode503()
    {
        var exception = new ServiceUnavailableException();

        exception.StatusCode.Should().Be(503);
        exception.Title.Should().Be("Service Unavailable");
    }

    [Fact]
    public void ServiceUnavailableException_WithMessage_StoresMessage()
    {
        const string message = "Under maintenance.";

        var exception = new ServiceUnavailableException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(503);
    }

    [Fact]
    public void ServiceUnavailableException_WithInnerException_PreservesInner()
    {
        const string message = "Service down.";
        var inner = new InvalidOperationException("Connection refused.");

        var exception = new ServiceUnavailableException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        exception.StatusCode.Should().Be(503);
    }

    // ── Non-HTTP Exceptions ────────────────────────────────────────────

    [Fact]
    public void EmptyOrNullArrayException_DefaultConstructor_UsesDefaultMessage()
    {
        var exception = new EmptyOrNullArrayException();

        exception.Message.Should().Be(ExceptionMessages.EmptyOrNullArray);
    }

    [Fact]
    public void EmptyOrNullArrayException_WithMessage_StoresMessage()
    {
        const string message = "Array is empty.";

        var exception = new EmptyOrNullArrayException(message);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void EmptyOrNullArrayException_WithInnerException_PreservesInner()
    {
        const string message = "Array error.";
        var inner = new InvalidOperationException("Source empty.");

        var exception = new EmptyOrNullArrayException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void EmptyOrNullCollectionException_DefaultConstructor_HasDefaultMessage()
    {
        var exception = new EmptyOrNullCollectionException();

        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void EmptyOrNullCollectionException_WithMessage_StoresMessage()
    {
        const string message = "Collection is empty.";

        var exception = new EmptyOrNullCollectionException(message);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void EmptyOrNullCollectionException_WithInnerException_PreservesInner()
    {
        const string message = "Collection error.";
        var inner = new InvalidOperationException("No items.");

        var exception = new EmptyOrNullCollectionException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void EmptyOrNullDictionaryException_DefaultConstructor_HasDefaultMessage()
    {
        var exception = new EmptyOrNullDictionaryException();

        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void EmptyOrNullDictionaryException_WithMessage_StoresMessage()
    {
        const string message = "Dictionary is empty.";

        var exception = new EmptyOrNullDictionaryException(message);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void EmptyOrNullDictionaryException_WithInnerException_PreservesInner()
    {
        const string message = "Dictionary error.";
        var inner = new InvalidOperationException("No entries.");

        var exception = new EmptyOrNullDictionaryException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void InsufficientDataException_WithMessage_StoresMessage()
    {
        const string message = "Not enough data points.";

        var exception = new InsufficientDataException(message);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void InsufficientDataException_WithInnerException_PreservesInner()
    {
        const string message = "Insufficient data.";
        var inner = new InvalidOperationException("Need at least 2 elements.");

        var exception = new InsufficientDataException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
    }
}
