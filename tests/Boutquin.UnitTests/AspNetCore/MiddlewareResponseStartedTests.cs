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
using Boutquin.Validation.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Tests for two previously-uncovered middleware paths: the response-already-started guard (which must
/// re-throw rather than corrupt a committed response) and the structured <see cref="ValidationException"/>
/// branch (which groups failures by property name into the <c>errors</c> extension).
/// </summary>
public sealed class MiddlewareResponseStartedTests
{
    // Minimal response feature whose HasStarted is always true, simulating a committed response.
    private sealed class StartedResponseFeature : IHttpResponseFeature
    {
        public int StatusCode { get; set; } = 200;
        public string? ReasonPhrase { get; set; }
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public Stream Body { get; set; } = Stream.Null;
        public bool HasStarted => true;
        public void OnStarting(Func<object, Task> callback, object state) { }
        public void OnCompleted(Func<object, Task> callback, object state) { }
    }

    [Fact]
    public async Task InvokeAsync_WhenResponseHasStarted_RethrowsOriginalException()
    {
        var context = new DefaultHttpContext();
        context.Features.Set<IHttpResponseFeature>(new StartedResponseFeature());
        var middleware = new CustomExceptionHandlerMiddleware(
            _ => throw new InvalidOperationException("original failure"));

        Func<Task> act = () => middleware.InvokeAsync(context);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("original failure");
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_Returns400WithGroupedErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required."),
            new("Name", "Name is too short."),
            new("Age", "Age must be positive."),
        };

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new CustomExceptionHandlerMiddleware(_ => throw new ValidationException(failures));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        problem!.Title.Should().Be("Validation Error");
        problem.Detail.Should().Be("One or more validation errors occurred.");
        problem.Extensions.Should().ContainKey("errors");

        // The "errors" extension groups messages by property name: "Name" carries two messages.
        var errors = (JsonElement)problem.Extensions["errors"]!;
        errors.GetProperty("Name").GetArrayLength().Should().Be(2);
        errors.GetProperty("Age").GetArrayLength().Should().Be(1);
    }
}
