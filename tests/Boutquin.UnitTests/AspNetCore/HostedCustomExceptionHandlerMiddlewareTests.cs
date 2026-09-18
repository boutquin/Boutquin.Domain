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

using System.Net;
using System.Text.Json;
using Boutquin.AspNetCore;
using Boutquin.Domain.Exceptions;
using Boutquin.Validation.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

/// <summary>
/// Exercises the exception middleware through its public registration extension and a real hosted HTTP pipeline.
/// </summary>
public sealed class HostedCustomExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task UseCustomExceptionHandler_WhenDownstreamThrowsValidationException_ReturnsProblemDetails()
    {
        await using var app = await CreateApplicationAsync(
            _ => throw new ValidationException(
            [
                new ValidationFailure("Name", "Name is required."),
                new ValidationFailure("Name", "Name is too short.")
            ]));

        using var response = await app.GetTestClient().GetAsync("/validation");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        document.RootElement.GetProperty("title").GetString().Should().Be("Validation Error");
        document.RootElement.GetProperty("detail").GetString().Should().Be("One or more validation errors occurred.");
        document.RootElement.GetProperty("errors").GetProperty("Name").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task UseCustomExceptionHandler_WhenDownstreamThrowsDomainException_ReturnsMappedProblemDetails()
    {
        await using var app = await CreateApplicationAsync(_ => throw new NotFoundException("Order 42 was not found."));

        using var response = await app.GetTestClient().GetAsync("/orders/42");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        document.RootElement.GetProperty("title").GetString().Should().Be("Not Found");
        document.RootElement.GetProperty("detail").GetString().Should().Be("Order 42 was not found.");
        document.RootElement.GetProperty("instance").GetString().Should().Be("/orders/42");
    }

    [Fact]
    public async Task UseCustomExceptionHandler_WhenDownstreamThrowsUnknownException_UsesDiLoggerAndSuppressesDetail()
    {
        var loggerProvider = new CapturingLoggerProvider();
        var exception = new InvalidOperationException("database password: not-for-client");

        await using var app = await CreateApplicationAsync(_ => throw exception, loggerProvider);

        using var response = await app.GetTestClient().GetAsync("/unknown");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        document.RootElement.GetProperty("title").GetString().Should().Be("Internal Server Error");
        document.RootElement.GetProperty("detail").GetString().Should().Be("An unexpected error occurred.");
        document.RootElement.GetRawText().Should().NotContain("not-for-client");
        loggerProvider.Entries.Should().ContainSingle(entry => entry.Level == LogLevel.Error && entry.Exception == exception);
    }

    private static async Task<WebApplication> CreateApplicationAsync(
        RequestDelegate downstream,
        ILoggerProvider? loggerProvider = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        if (loggerProvider is not null)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(loggerProvider);
        }

        var app = builder.Build();
        app.UseCustomExceptionHandler();
        app.Run(downstream);
        await app.StartAsync();
        return app;
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public List<(LogLevel Level, Exception? Exception)> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(this);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(CapturingLoggerProvider provider) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
                => provider.Entries.Add((logLevel, exception));
        }
    }
}
