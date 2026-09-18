# Boutquin.AspNetCore

ASP.NET Core integration for Boutquin.Domain: RFC 7807 exception handling and
convention-based module registration for modular applications.

## Install

```bash
dotnet add package Boutquin.AspNetCore
```

## Configure an application

Register modules before building the application, then place the exception handler
before the endpoints it should protect.

```csharp
using Boutquin.AspNetCore;
using Boutquin.AspNetCore.Extensions;

builder.Services.RegisterModules();

var app = builder.Build();
app.UseCustomExceptionHandler();
app.MapEndpoints();
app.Run();
```

An `IModule` implements both `RegisterModule(IServiceCollection)` and
`MapEndpoints(IEndpointRouteBuilder)`. `RegisterModules` can also receive explicit
assemblies when convention-based discovery is not appropriate.

The middleware writes `application/problem+json` responses. It maps
`ValidationException` to 400 with validation failures grouped by property name, and
maps `DomainException` subclasses according to their configured HTTP status. It
does not rewrite a response that has already started, and it lets client-abort
`OperationCanceledException` instances propagate.

## Documentation

See the [middleware guide](https://github.com/boutquin/Boutquin.Domain/blob/main/src/Boutquin.AspNetCore/doc/CustomExceptionHandlerMiddleware.md)
and [module-registration guide](https://github.com/boutquin/Boutquin.Domain/blob/main/src/Boutquin.AspNetCore/doc/ModuleExtensions.md).

## License

Apache-2.0. See the [repository license](https://github.com/boutquin/Boutquin.Domain/blob/main/LICENSE.txt).
