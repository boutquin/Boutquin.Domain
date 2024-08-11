// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.AspNetCore.Abstractions;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a module that can be registered and mapped to endpoints in an ASP.NET Core application.
/// </summary>
/// <example>
/// This sample shows how to implement this interface and use its methods.
/// <code>
/// public sealed class MyModule : IModule
/// {
///     public IServiceCollection RegisterModule(IServiceCollection builder)
///     {
///         // Register services specific to this module
///         return builder;
///     }
///
///     public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
///     {
///         // Map endpoints specific to this module
///         return endpoints;
///     }
/// }
/// </code>
/// </example>
public interface IModule
{
    /// <summary>
    /// Registers the module's services with the provided service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    IServiceCollection RegisterModule(IServiceCollection builder);

    /// <summary>
    /// Maps the module's endpoints with the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the endpoints to.</param>
    /// <returns>The same endpoint route builder so that multiple calls can be chained.</returns>
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
