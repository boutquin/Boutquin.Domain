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

