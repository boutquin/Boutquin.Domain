// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.AspNetCore;

using Domain.Helpers;
using Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to add the <see cref="CustomExceptionHandlerMiddleware"/> to the application pipeline.
/// </summary>
public static class CustomExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// Adds a middleware type to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> passed to the Configure method.</param>
    /// <returns>An updated <see cref="IApplicationBuilder"/> with the middleware added.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="app"/> is null.</exception>
    /// <example>
    /// This sample shows how to use the <see cref="UseCustomExceptionHandler"/> method.
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     if (env.IsDevelopment())
    ///     {
    ///         app.UseDeveloperExceptionPage();
    ///     }
    ///     else
    ///     {
    ///         app.UseCustomExceptionHandler();
    ///     }
    ///
    ///     // ... Other middleware registrations
    /// }
    /// </code>
    /// </example>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        Guard.AgainstNull(() => app);

        return app.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}

