# Class Name: ModuleExtensions

**Namespace:** `Boutquin.AspNetCore.Extensions`

Static class providing extension methods for module discovery, registration, and endpoint mapping in ASP.NET Core applications following the modular monolith pattern.

## Design

The modular monolith pattern encapsulates each vertical slice of the application in a module that self-registers its services and endpoints, avoiding a monolithic `Startup` class. Modules implement the `IModule` interface and are discovered via assembly scanning.

### Testability Design

The public overload delegates to an `internal` overload that accepts a `Func<Assembly>` assembly resolver. This two-layer design exists because `Assembly.GetEntryAssembly()` and `Assembly.GetCallingAssembly()` are static methods whose return values depend on the runtime call stack and cannot be controlled in unit tests. The `internal` overload is exposed to the test project via `InternalsVisibleTo`.

## IModule Interface

**Namespace:** `Boutquin.AspNetCore.Abstractions`

```csharp
public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection builder);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
```

## Methods

### `IServiceCollection RegisterModules(this IServiceCollection services, params Assembly[] assemblies)`

Discovers all `IModule` implementations in the specified assemblies (or the entry assembly by default), calls `RegisterModule` on each, and stores the module list as a DI singleton.

**Parameters:**
- `services` (IServiceCollection): The service collection.
- `assemblies` (Assembly[]): Assemblies to scan. If empty, defaults to the entry assembly.

**Returns:** The same `IServiceCollection` for chaining.

### `internal IServiceCollection RegisterModules(this IServiceCollection services, Func<Assembly>? assemblyResolver, params Assembly[] assemblies)`

Internal overload for testability. When `assemblies` is empty and `assemblyResolver` is provided, uses the resolver to determine which assembly to scan.

### `WebApplication MapEndpoints(this WebApplication app)`

Retrieves the registered module list from DI and calls `MapEndpoints` on each module. Must be called after `RegisterModules`.

**Parameters:**
- `app` (WebApplication): The application builder.

**Returns:** The same `WebApplication` for chaining.

## Usage

```csharp
// In Program.cs or Startup:
builder.Services.RegisterModules();    // scans entry assembly
// or:
builder.Services.RegisterModules(typeof(MyModule).Assembly);

var app = builder.Build();
app.MapEndpoints();

// Module implementation:
public sealed class OrderModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/orders", GetOrders);
        return endpoints;
    }
}
```
