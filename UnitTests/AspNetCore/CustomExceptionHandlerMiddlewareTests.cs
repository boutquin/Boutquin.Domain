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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    [Fact]
    public async Task InvokeAsync_WhenInternalServerErrorException_Returns500()
    {
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new InternalServerErrorException("Server error"));

        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Detail.Should().Be("Server error");
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
    public async Task InvokeAsync_WhenUnknownException_Returns500WithGenericMessage()
    {
        var (statusCode, problemDetails) = await InvokeMiddlewareWithException(
            new InvalidOperationException("Something went wrong"));

        statusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("Internal Server Error");
        problemDetails.Detail.Should().Be("An unexpected error occurred.");
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
