# Architecture

This document explains how the components in Boutquin.Domain fit together. For a feature overview, see [README.md](README.md).

## Layers

The solution contains three independently-packaged libraries plus a test project:

```
Domain/        Core DDD building blocks (zero external dependencies)
Validation/    FluentValidation integration (depends on Domain)
AspNetCore/    Middleware and module system (depends on Domain + Validation)
UnitTests/     xUnit tests (depends on all three)
```

Domain has zero external dependencies. Validation depends only on Domain and FluentValidation. AspNetCore depends on Domain, Validation, and `Microsoft.AspNetCore.App`. Tests depend on all three.

## Core Abstractions

### Entity & Identity

```
IEntity
  └─ Entity<TEntityId>          Identity-based equality, domain event buffering
       │
       └─ StronglyTypedId<T>    Wraps primitives as domain-specific IDs
```

**Why identity-based equality?** Domain entities are identified by their ID, not their properties. Two `Order` objects with the same `OrderId` are the same order, regardless of other field values. `Entity<TEntityId>` enforces this by using only `Id` in `Equals`/`GetHashCode`. Transient entities (default ID) throw on comparison to prevent accidental identity collisions.

### Result Pattern

```
Result                          Success/failure without exceptions
  └─ Result<TValue>             Typed success value
       │
       ├─ implicit TValue       Auto-wrap success values
       └─ implicit Error        Auto-wrap errors

Error(Code, Name, ErrorType)    Value object with HTTP status mapping
  │
  ├─ Error.None                 Sentinel for "no error"
  ├─ Error.NullValue            Sentinel for null arguments
  └─ Factory methods            BadRequest(), NotFound(), Conflict(), etc.

ErrorType                       Enum mapped to HttpStatusCode values
```

**Why Result instead of exceptions?** Exceptions are for exceptional situations. Domain validation failures, not-found lookups, and conflict checks are expected outcomes. The Result pattern makes failure explicit in the type signature, forcing callers to handle both paths. Implicit conversions reduce ceremony: `return order;` for success, `return Error.NotFound(...)` for failure.

### Result Extensions

```
ResultExtensions
  ├─ Match / MatchAsync         Pattern match on success/failure
  ├─ Map                        Transform success value (TIn → TOut)
  ├─ Bind / BindAsync           Chain Result-returning operations
  ├─ Tap / TapAsync             Side-effect on success (logging, events)
  └─ OnFailure                  Side-effect on failure (logging, metrics)
```

These compose into pipelines:

```csharp
await GetOrder(id)
    .Map(order => order.ToDto())
    .Tap(dto => logger.LogInformation("Found order {Id}", dto.Id))
    .OnFailure(error => logger.LogWarning("Order lookup failed: {Code}", error.Code))
    .MatchAsync(
        dto => Results.Ok(dto),
        error => Results.Problem(error));
```

### Domain Events

```
INotification                   Marker interface (no MediatR dependency)
  └─ IDomainEvent               Domain event marker

Entity<TEntityId>
  ├─ RaiseDomainEvent()         Buffers events during business logic
  ├─ GetDomainEvents()          Returns buffered events
  └─ ClearDomainEvents()        Clears after dispatch
```

**Why no MediatR dependency?** The domain layer should not depend on infrastructure concerns. `INotification` is a simple marker interface. The infrastructure layer (your mediator of choice) dispatches events collected from entities after persistence.

### Guard Clauses

```
Guard
  ├─ CallerArgumentExpression API    Guard.AgainstNullOrEmpty(param)
  │   (C# 10+ — auto-captures parameter name at compile time)
  │
  └─ Expression-based API           Guard.AgainstNullOrEmpty(() => param)
      (Legacy — extracts name from expression tree at runtime)
```

Both APIs throw `ArgumentException` subtypes. The `CallerArgumentExpression` API is preferred for new code (zero runtime overhead). The expression-based API exists for backward compatibility.

Guard methods are `internal` with `InternalsVisibleTo` for the test project. This keeps the public API surface clean while allowing thorough testing.

## ASP.NET Core Integration

### Exception Middleware

```
HTTP Request
  │
  ├─ CustomExceptionHandlerMiddleware
  │     │
  │     ├─ ValidationException     → 400 Bad Request (grouped errors)
  │     ├─ NotFoundException       → 404 Not Found
  │     ├─ ConflictException       → 409 Conflict
  │     ├─ ForbiddenException      → 403 Forbidden
  │     ├─ UnauthorizedException   → 401 Unauthorized
  │     └─ (unknown)               → 500 Internal Server Error
  │
  └─ Response: application/problem+json (RFC 7807)
```

### Module System

```
IModule
  ├─ RegisterModule(services)      Register DI services
  └─ MapEndpoints(app)             Map HTTP endpoints

ModuleExtensions
  ├─ RegisterModules(services)     Discovers IModule implementations in entry assembly
  └─ MapEndpoints(app)             Calls MapEndpoints on all discovered modules
```

The `RegisterModules` overload accepts `Func<Assembly>` for testability — `Assembly.GetEntryAssembly()` returns null under xUnit. This overload is internal (exposed via `InternalsVisibleTo`).

## Validation

```
FluentValidation.AbstractValidator<T>
  │
  └─ ValidationBehavior<TRequest, TResponse>    Pipeline behavior (mediator integration)
       │
       └─ throws ValidationException            Caught by middleware → 400
            │
            └─ Errors: IReadOnlyDictionary<string, string[]>
                 (property name → error messages, grouped)
```

## Component Navigation

**"I want to..."** → Start here:

| Goal | Start at | Key files |
|------|----------|-----------|
| Define a domain entity | `Entity<TEntityId>` | `Domain/Abstractions/Entity.cs` |
| Create a strongly typed ID | `StronglyTypedId<T>` | `Domain/Abstractions/StronglyTypedId.cs` |
| Return success/failure | `Result<T>` | `Domain/Abstractions/Result.cs`, `Result{TValue}.cs` |
| Chain Result operations | `ResultExtensions` | `Domain/Extensions/ResultExtensions.cs` |
| Define error types | `Error` + `ErrorType` | `Domain/Abstractions/Error.cs`, `ErrorType.cs` |
| Validate parameters | `Guard` | `Domain/Helpers/Guard.cs` |
| Handle exceptions as HTTP | `CustomExceptionHandlerMiddleware` | `AspNetCore/Middleware/` |
| Register modules | `ModuleExtensions` | `AspNetCore/Extensions/ModuleExtensions.cs` |
| Validate with FluentValidation | `ValidationException` | `Validation/ValidationException.cs` |
| Raise domain events | `Entity.RaiseDomainEvent()` | `Domain/Abstractions/Entity.cs` |

## Directory Structure

```
Domain/
  Abstractions/        Entity, Result, Error, ErrorType, INotification, IDomainEvent,
                       IEntity, IUnitOfWork, StronglyTypedId
  Helpers/             Guard, GuardCondition
  Extensions/          StringExtensions, DateTimeExtensions, EnumExtensions,
                       ResultExtensions, JsonElementExtensions, DecimalArrayExtensions
  Converters/          DateOnlyConverter, DateOnlyDictionaryConverterFactory
  Exceptions/          DomainException hierarchy (maps to HTTP status codes)
  doc/                 Per-component API documentation
AspNetCore/
  Middleware/          CustomExceptionHandlerMiddleware
  Extensions/          ModuleExtensions
  doc/                 Middleware and module documentation
Validation/
  ValidationException  FluentValidation error wrapper
  doc/                 Validation documentation
UnitTests/             308 xUnit tests with FluentAssertions
```
