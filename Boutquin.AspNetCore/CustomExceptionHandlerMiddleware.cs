// Copyright (c) 2023-2024 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
namespace Boutquin.AspNetCore;

using System.Net;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Validation.Exceptions;

/// <summary>
/// A custom exception handler middleware that catches exceptions, maps them to the appropriate HTTP status codes, and creates ProblemDetails responses.
/// </summary>
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
    /// Handles exceptions by mapping them to the appropriate HTTP status codes and creating ProblemDetails responses.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request and response.</param>
    /// <param name="exception">The <see cref="Exception"/> that was thrown.</param>
    /// <returns>A <see cref="Task"/> that represents the handling of the exception.</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Initialize the response status code and ProblemDetails object
        var statusCode = StatusCodes.Status500InternalServerError;
        var problemDetails = new ProblemDetails();

        // Map each custom exception to its corresponding status code and update problemDetails
        switch (exception)
        {
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

            case BadRequestException badRequestException:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = badRequestException.Message;
                break;

            case UnauthorizedException unauthorizedException:
                statusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = unauthorizedException.Message;
                break;

            case ForbiddenException forbiddenException:
                statusCode = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Forbidden";
                problemDetails.Detail = forbiddenException.Message;
                break;

            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Detail = notFoundException.Message;
                break;

            case ConflictException conflictException:
                statusCode = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflict";
                problemDetails.Detail = conflictException.Message;
                break;

            case UnsupportedMediaTypeException unsupportedMediaTypeException:
                statusCode = StatusCodes.Status415UnsupportedMediaType;
                problemDetails.Title = "Unsupported Media Type";
                problemDetails.Detail = unsupportedMediaTypeException.Message;
                break;

            case UnprocessableEntityException unprocessableEntityException:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "Unprocessable Entity";
                problemDetails.Detail = unprocessableEntityException.Message;
                break;

            case TooManyRequestsException tooManyRequestsException:
                statusCode = StatusCodes.Status429TooManyRequests;
                problemDetails.Title = "Too Many Requests";
                problemDetails.Detail = tooManyRequestsException.Message;
                break;

            default:
                // Keep the default status code (500) and update the problemDetails for unknown exceptions
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
                break;
        }

        // Set the status code and instance
        problemDetails.Status = statusCode;
        problemDetails.Instance = context.Request.Path;

        // Set the response status code and content type
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        // Serialize the problemDetails object and write it to the response
        await context.Response.WriteAsJsonAsync(problemDetails).ConfigureAwait(false);
    }
}
