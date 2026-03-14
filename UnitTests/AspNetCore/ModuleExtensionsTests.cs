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

namespace Boutquin.UnitTests.AspNetCore;

using System.Reflection;
using Boutquin.AspNetCore.Abstractions;
using Boutquin.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains unit tests for the <see cref="ModuleExtensions"/> class.
/// </summary>
/// <remarks>
/// <para>
/// These tests verify the module discovery, registration, and endpoint mapping pipeline.
/// The key challenge is testing the assembly resolution fallback branch — the code path
/// that activates when <c>RegisterModules()</c> is called without explicit assemblies.
/// </para>
/// <para>
/// <b>The testability problem:</b> The original fallback used
/// <c>Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()</c>. Both are static
/// methods whose return values depend on the runtime call stack:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       <c>GetEntryAssembly()</c> returns <c>null</c> under xUnit because the test runner
///       (not the application) is the managed entry point.
///     </description>
///   </item>
///   <item>
///     <description>
///       <c>GetCallingAssembly()</c> returns Boutquin.AspNetCore (the assembly containing
///       <c>RegisterModules</c>), not the test assembly — because the public overload
///       delegates to the internal overload, making Boutquin.AspNetCore the immediate caller.
///     </description>
///   </item>
/// </list>
/// <para>
/// <b>The solution:</b> An <c>internal</c> overload of <c>RegisterModules</c> accepts a
/// <c>Func&lt;Assembly&gt;</c> parameter that replaces the static assembly resolution.
/// Tests inject a resolver that returns the test assembly (which contains <see cref="TestModule"/>),
/// while the public overload passes <c>null</c> to use the default strategy in production.
/// The <c>internal</c> overload is accessible here via <c>InternalsVisibleTo</c> in the
/// Boutquin.AspNetCore <c>.csproj</c>.
/// </para>
/// </remarks>
public sealed class ModuleExtensionsTests
{
    /// <summary>
    /// A test module that tracks whether its methods have been called.
    /// </summary>
    /// <remarks>
    /// Uses static flags because <c>DiscoverModules</c> creates instances via
    /// <c>Activator.CreateInstance</c> — we can't inject or capture the instance,
    /// so static state is the only way to observe that the module was activated.
    /// Each test resets the flags in its Arrange phase to ensure isolation.
    /// </remarks>
    public sealed class TestModule : IModule
    {
        public static bool RegisterModuleCalled { get; set; }
        public static bool MapEndpointsCalled { get; set; }

        public IServiceCollection RegisterModule(IServiceCollection builder)
        {
            RegisterModuleCalled = true;
            return builder;
        }

        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            MapEndpointsCalled = true;
            return endpoints;
        }
    }

    /// <summary>
    /// Verifies the happy path: passing an explicit assembly discovers and registers
    /// the modules it contains, and stores them in the DI container.
    /// </summary>
    [Fact]
    public void RegisterModules_WithAssembly_DiscoversAndRegistersModules()
    {
        // Arrange
        TestModule.RegisterModuleCalled = false;
        var services = new ServiceCollection();

        // Act — pass the test assembly explicitly, bypassing the fallback branch entirely
        var result = services.RegisterModules(Assembly.GetExecutingAssembly());

        // Assert
        result.Should().BeSameAs(services);
        TestModule.RegisterModuleCalled.Should().BeTrue();

        // Verify the module list was registered as a singleton so MapEndpoints can find it
        var provider = services.BuildServiceProvider();
        var modules = provider.GetRequiredService<IReadOnlyList<IModule>>();
        modules.Should().ContainSingle(m => m is TestModule);
    }

    /// <summary>
    /// Verifies that <see cref="ModuleExtensions.MapEndpoints"/> retrieves the registered
    /// modules and calls <see cref="IModule.MapEndpoints"/> on each one.
    /// </summary>
    [Fact]
    public void MapEndpoints_CallsMapEndpointsOnAllRegisteredModules()
    {
        // Arrange
        TestModule.MapEndpointsCalled = false;
        TestModule.RegisterModuleCalled = false;
        var builder = WebApplication.CreateBuilder();
        builder.Services.RegisterModules(Assembly.GetExecutingAssembly());
        var app = builder.Build();

        // Act
        var result = app.MapEndpoints();

        // Assert
        result.Should().BeSameAs(app);
        TestModule.MapEndpointsCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies the fluent API contract: <c>RegisterModules</c> returns the same
    /// <see cref="IServiceCollection"/> to allow method chaining.
    /// </summary>
    [Fact]
    public void RegisterModules_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.RegisterModules(Assembly.GetExecutingAssembly());

        // Assert
        result.Should().BeSameAs(services);
    }

    /// <summary>
    /// Verifies the assembly resolver fallback branch by injecting a custom resolver.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This test exercises the <c>internal</c> overload of <c>RegisterModules</c> that accepts
    /// a <c>Func&lt;Assembly&gt;</c> parameter. By passing no assemblies and providing a resolver,
    /// we force the code into the <c>if (assemblies.Length == 0)</c> branch and verify that the
    /// resolver is invoked to determine which assembly to scan.
    /// </para>
    /// <para>
    /// <b>Critical subtlety — why we capture the assembly in a variable:</b>
    /// </para>
    /// <para>
    /// The naive approach would be to pass <c>Assembly.GetExecutingAssembly</c> as a method group:
    /// <code>
    /// services.RegisterModules(Assembly.GetExecutingAssembly);  // WRONG
    /// </code>
    /// This fails because <c>GetExecutingAssembly</c> is evaluated when the delegate is
    /// <em>invoked</em>, not when it is <em>created</em>. The delegate is invoked inside
    /// <c>ModuleExtensions.RegisterModules</c> (in the Boutquin.AspNetCore assembly), so
    /// <c>GetExecutingAssembly</c> returns Boutquin.AspNetCore — which does not contain
    /// <c>TestModule</c>.
    /// </para>
    /// <para>
    /// The correct approach captures the assembly eagerly in the test method (where
    /// <c>GetExecutingAssembly</c> returns the test assembly) and closes over the captured value:
    /// <code>
    /// var testAssembly = Assembly.GetExecutingAssembly();  // resolves to test assembly HERE
    /// services.RegisterModules(() => testAssembly);        // closure returns the captured value
    /// </code>
    /// This way, no matter where the delegate is invoked, it always returns the test assembly.
    /// </para>
    /// </remarks>
    [Fact]
    public void RegisterModules_WithNoAssemblies_UsesResolverFallback()
    {
        // Arrange
        TestModule.RegisterModuleCalled = false;
        var services = new ServiceCollection();

        // Capture the test assembly eagerly. Assembly.GetExecutingAssembly() is called HERE,
        // in the test assembly, so it returns Boutquin.UnitTests — the assembly containing
        // TestModule. We close over this captured value in the lambda below.
        var testAssembly = Assembly.GetExecutingAssembly();

        // Act — call the internal overload with:
        //   - A resolver that returns our captured test assembly
        //   - No explicit assemblies (empty params), which triggers the fallback branch
        var result = services.RegisterModules(
            () => testAssembly);

        // Assert — the resolver was used, TestModule was discovered and registered
        result.Should().BeSameAs(services);
        TestModule.RegisterModuleCalled.Should().BeTrue();

        var provider = services.BuildServiceProvider();
        var modules = provider.GetRequiredService<IReadOnlyList<IModule>>();
        modules.Should().ContainSingle(m => m is TestModule);
    }

    /// <summary>
    /// Verifies that passing a <c>null</c> resolver triggers the default assembly resolution
    /// strategy without throwing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This test covers the <c>null</c> path of the null-coalescing operator in the internal
    /// overload: <c>assemblyResolver ?? (() => Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())</c>.
    /// </para>
    /// <para>
    /// Under xUnit, <c>GetEntryAssembly()</c> returns <c>null</c> and <c>GetCallingAssembly()</c>
    /// returns the xUnit runner assembly, which contains no <see cref="IModule"/> implementations.
    /// So we cannot assert that <c>TestModule</c> was discovered — we can only verify that the
    /// method completes without error and registers a (possibly empty) module list. This is
    /// intentional: we are testing that the default fallback path executes, not that it discovers
    /// specific modules (which depends on the runtime environment).
    /// </para>
    /// <para>
    /// The explicit cast <c>(Func&lt;Assembly&gt;?)null</c> is required because without it, the
    /// compiler cannot disambiguate between the public <c>params Assembly[]</c> overload and the
    /// internal <c>Func&lt;Assembly&gt;?, params Assembly[]</c> overload — both accept <c>null</c>
    /// as a valid argument.
    /// </para>
    /// </remarks>
    [Fact]
    public void RegisterModules_WithNoAssembliesAndNullResolver_UsesDefaultFallback()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act — pass null resolver explicitly (cast needed to disambiguate overloads)
        // This triggers the default GetEntryAssembly() ?? GetCallingAssembly() chain
        var result = services.RegisterModules((Func<Assembly>?)null);

        // Assert — the method completes without error and registers a module list.
        // We don't assert on module contents because the default resolver returns
        // an assembly we don't control in the test environment.
        result.Should().BeSameAs(services);
        var provider = services.BuildServiceProvider();
        var modules = provider.GetRequiredService<IReadOnlyList<IModule>>();
        modules.Should().NotBeNull();
    }
}
