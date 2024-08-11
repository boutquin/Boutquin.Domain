// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
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

namespace Boutquin.AspNetCore.Extensions;

using Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering and mapping modules in an ASP.NET Core application.
/// </summary>
/// <example>
/// This sample shows how to use these methods in an application's startup code.
/// <code>
/// public sealed class Startup
/// {
///     public void ConfigureServices(IServiceCollection services)
///     {
///         services.RegisterModules();
///     }
///
///     public void Configure(WebApplication app)
///     {
///         app.MapEndpoints();
///     }
/// }
/// </code>
/// </example>
public static class ModuleExtensions
{
    // this could also be added into the DI container
    static readonly List<IModule> s_registeredModules = new();

    /// <summary>
    /// Discovers and registers all modules in the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            module.RegisterModule(services);
            s_registeredModules.Add(module);
        }

        return services;
    }

    /// <summary>
    /// Maps the endpoints of all registered modules in the application.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to add the endpoints to.</param>
    /// <returns>The same application builder so that multiple calls can be chained.</returns>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in s_registeredModules)
        {
            module.MapEndpoints(app);
        }
        return app;
    }

    /// <summary>
    /// Discovers all modules in the application.
    /// </summary>
    /// <returns>An enumerable of all discovered modules.</returns>
    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}
