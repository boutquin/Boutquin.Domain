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

namespace Boutquin.AspNetCore.Extensions;

using System.Reflection;
using Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering and mapping modules in an ASP.NET Core application.
/// </summary>
/// <remarks>
/// <para>
/// This class implements the modular monolith pattern, where each vertical slice of the application
/// is encapsulated in a module that self-registers its services and endpoints. This avoids a
/// single monolithic <c>Startup</c> class that grows unboundedly as features are added.
/// </para>
/// <para>
/// <b>Testability design:</b> The public <see cref="RegisterModules(IServiceCollection, Assembly[])"/>
/// overload delegates to an <c>internal</c> overload that accepts an injectable <see cref="Func{Assembly}"/>
/// assembly resolver. This two-layer design exists because <see cref="Assembly.GetEntryAssembly"/> and
/// <see cref="Assembly.GetCallingAssembly"/> are static methods whose return values depend on the runtime
/// call stack — they cannot be mocked or controlled in unit tests. By extracting the assembly resolution
/// into a delegate parameter, we allow tests to inject a known assembly while keeping the public API
/// clean and unchanged for production callers.
/// </para>
/// <para>
/// The <c>internal</c> overload is exposed to the test project via <c>InternalsVisibleTo</c> in the
/// <c>.csproj</c> file, which grants the test assembly access without polluting the public API surface.
/// </para>
/// </remarks>
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
    /// <summary>
    /// Discovers and registers all modules found in the specified assemblies (or the entry assembly by default),
    /// and stores them in the DI container as a singleton list for later endpoint mapping.
    /// </summary>
    /// <remarks>
    /// This is the public-facing entry point. It delegates to the <c>internal</c> overload with a
    /// <c>null</c> assembly resolver, which causes the internal method to use its default resolution
    /// strategy (<see cref="Assembly.GetEntryAssembly"/> ?? <see cref="Assembly.GetCallingAssembly"/>).
    /// The delegation exists solely to enable testability — see the class-level remarks for the full
    /// rationale behind the two-layer design.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assemblies">
    /// The assemblies to scan for <see cref="IModule"/> implementations.
    /// If none are provided, the entry assembly is scanned.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection RegisterModules(this IServiceCollection services, params Assembly[] assemblies)
        => RegisterModules(services, null, assemblies);

    /// <summary>
    /// Discovers and registers all modules found in the specified assemblies,
    /// using the provided assembly resolver as a fallback when no assemblies are specified.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why this overload exists:</b> The original code used
    /// <c>Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()</c> as a convenience fallback
    /// when no assemblies were passed. However, both of these methods are static with behavior
    /// determined by the runtime call stack:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <see cref="Assembly.GetEntryAssembly"/> returns <c>null</c> when there is no managed
    ///       entry point — which is always the case under test runners like xUnit, since the entry
    ///       point is the test host, not the application.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <see cref="Assembly.GetCallingAssembly"/> returns the assembly of the method that
    ///       <em>invoked</em> the current method. When this internal overload is called from the
    ///       public overload (which lives in the Boutquin.AspNetCore assembly), the "calling assembly"
    ///       is Boutquin.AspNetCore — not the test assembly. So even though the test triggers the
    ///       fallback branch, the resolver scans the wrong assembly and finds no test modules.
    ///     </description>
    ///   </item>
    /// </list>
    /// <para>
    /// By accepting a <see cref="Func{Assembly}"/> parameter, tests can inject a resolver that returns
    /// a specific assembly (e.g., <c>Assembly.GetExecutingAssembly()</c> captured in the test method,
    /// which resolves to the test assembly containing the test <see cref="IModule"/> implementations).
    /// </para>
    /// <para>
    /// <b>Why <c>internal</c> and not <c>public</c>:</b> This overload is an implementation detail
    /// that exists only to support testability. Making it <c>public</c> would add an overload to the
    /// API surface that production callers never need and shouldn't be tempted to use. The
    /// <c>internal</c> visibility, combined with <c>InternalsVisibleTo</c> in the <c>.csproj</c>,
    /// limits access to the test project only.
    /// </para>
    /// <para>
    /// <b>Why not a full <c>IAssemblyProvider</c> interface:</b> An interface + DI registration would
    /// be the textbook approach for larger seams, but here the only consumer of this seam is one
    /// fallback branch in one method. A <see cref="Func{Assembly}"/> parameter achieves the same
    /// result with zero new types, zero DI registrations, and zero ceremony. The trade-off favors
    /// simplicity given the narrow scope of the problem.
    /// </para>
    /// <para>
    /// <b>Important subtlety — capturing vs. method groups:</b> When passing
    /// <c>Assembly.GetExecutingAssembly</c> as a method group (i.e., without the <c>()</c> invocation),
    /// the delegate is resolved at invocation time — meaning it returns the assembly where the
    /// delegate is <em>invoked</em> (inside ModuleExtensions), not where it was <em>created</em>
    /// (in the test). To get the correct assembly, tests must capture the assembly into a local
    /// variable first:
    /// <code>
    /// var testAssembly = Assembly.GetExecutingAssembly();   // captured here, in the test assembly
    /// services.RegisterModules(() => testAssembly);         // closure returns the captured value
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assemblyResolver">
    /// An optional factory that returns the default assembly to scan when <paramref name="assemblies"/> is empty.
    /// When <c>null</c>, defaults to <see cref="Assembly.GetEntryAssembly"/> ?? <see cref="Assembly.GetCallingAssembly"/>.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies to scan for <see cref="IModule"/> implementations.
    /// If none are provided, the <paramref name="assemblyResolver"/> is used.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    internal static IServiceCollection RegisterModules(
        this IServiceCollection services,
        Func<Assembly>? assemblyResolver,
        params Assembly[] assemblies)
    {
        // When the caller passes explicit assemblies, the resolver is irrelevant — we scan
        // exactly what was requested. The fallback branch only activates for the parameterless
        // convenience call (e.g., services.RegisterModules()).
        if (assemblies.Length == 0)
        {
            // Use the injected resolver if provided; otherwise fall back to the default strategy.
            // The null-coalescing here is what makes the public overload work transparently:
            // it passes null, which triggers the default GetEntryAssembly/GetCallingAssembly chain.
            var resolver = assemblyResolver
                ?? (() => Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly());
            assemblies = [resolver()];
        }

        var modules = DiscoverModules(assemblies);
        var registeredModules = new List<IModule>();

        foreach (var module in modules)
        {
            module.RegisterModule(services);
            registeredModules.Add(module);
        }

        // Store the discovered modules as a singleton so MapEndpoints can retrieve them later
        // without re-scanning assemblies. This creates a two-phase initialization pattern:
        // 1. RegisterModules() during ConfigureServices — discovers and registers
        // 2. MapEndpoints() during Configure — retrieves and maps
        services.AddSingleton<IReadOnlyList<IModule>>(registeredModules);

        return services;
    }

    /// <summary>
    /// Maps the endpoints of all registered modules in the application.
    /// </summary>
    /// <remarks>
    /// This method retrieves the module list that was stored during
    /// <see cref="RegisterModules(IServiceCollection, Assembly[])"/> and calls
    /// <see cref="IModule.MapEndpoints"/> on each one. It must be called after
    /// <c>RegisterModules</c>; calling it without prior registration will throw
    /// <see cref="InvalidOperationException"/> from <c>GetRequiredService</c>.
    /// </remarks>
    /// <param name="app">The <see cref="WebApplication"/> to add the endpoints to.</param>
    /// <returns>The same application builder so that multiple calls can be chained.</returns>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var modules = app.Services.GetRequiredService<IReadOnlyList<IModule>>();
        foreach (var module in modules)
        {
            module.MapEndpoints(app);
        }
        return app;
    }

    /// <summary>
    /// Discovers all modules in the specified assemblies.
    /// </summary>
    /// <remarks>
    /// Scans the provided assemblies for concrete (non-abstract) classes that implement
    /// <see cref="IModule"/>, then creates an instance of each using its parameterless constructor.
    /// Modules must have a public parameterless constructor or <see cref="Activator.CreateInstance(Type)"/>
    /// will throw.
    /// </remarks>
    /// <param name="assemblies">The assemblies to scan for <see cref="IModule"/> implementations.</param>
    /// <returns>An enumerable of all discovered modules.</returns>
    private static IEnumerable<IModule> DiscoverModules(Assembly[] assemblies)
    {
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(p => p.IsClass && !p.IsAbstract && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}
