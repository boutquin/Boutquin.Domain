# Boutquin.Domain

![NuGet](https://img.shields.io/nuget/vpre/Boutquin.Domain?style=for-the-badge) ![License](https://img.shields.io/github/license/boutquin/Boutquin.Domain?style=for-the-badge)

A .NET 10 library family for domain-driven design building blocks, FluentValidation failures, and ASP.NET Core exception handling.

## Solution Structure

The solution contains three library projects and a test project:

| Project | NuGet Package | Description |
|---------|---------------|-------------|
| **Domain** (`Boutquin.Domain`) | [`Boutquin.Domain`](https://www.nuget.org/packages/Boutquin.Domain) | Core domain abstractions, guards, value objects, extensions, converters, and exceptions |
| **Boutquin.AspNetCore** | [`Boutquin.AspNetCore`](https://www.nuget.org/packages/Boutquin.AspNetCore) | RFC 7807 exception middleware and modular endpoint registration |
| **Boutquin.Validation** | [`Boutquin.Validation`](https://www.nuget.org/packages/Boutquin.Validation) | FluentValidation integration and `ValidationException` |
| **UnitTests** (`Boutquin.UnitTests`) | — | xUnit coverage for all three packages |

## Domain Project

### Abstractions

The building blocks for Domain-Driven Design:

- **[Entity&lt;TEntityId&gt;](src/Boutquin.Domain/doc/Entity.md)** — Abstract base class with identity-based equality, domain event buffering, and ORM-compatible constructors.
- **[Result / Result&lt;TValue&gt;](src/Boutquin.Domain/doc/Result.md)** — Functional error handling — return success/failure instead of throwing exceptions.
- **[Error](src/Boutquin.Domain/doc/Result.md)** — Immutable record representing a domain error with code and description.
- **IEntity** — Interface for entities that generate domain events.
- **IDomainEvent** — Marker interface for domain events (implements `INotification`).
- **IUnitOfWork** — Defines the persistence boundary (`SaveChangesAsync`).

### Helpers

- **[Guard](src/Boutquin.Domain/doc/Guard.md)** — Static utility class for parameter validation with two API styles: expression-based (auto-extracts parameter names) and `CallerArgumentExpression`-based (zero overhead).
- **[GuardCondition](src/Boutquin.Domain/doc/GuardCondition.md)** — Fluent chaining helper for `Guard.Against().With<TException>()` and constructor-argument `WithArgs<TException>()` calls.
- **[StronglyTypedId&lt;TValue&gt;](src/Boutquin.Domain/doc/StronglyTypedId.md)** — Abstract record for wrapping primitives as domain-specific ID types, preventing primitive obsession.

### Extensions

- **[StringExtensions](src/Boutquin.Domain/doc/StringExtensions.md)** — `IsNullOrEmpty`, `IsNullOrWhiteSpace`, `ToUpperCaseFirst`, `ToLowerCaseFirst`, `Compare`, `CompareOrdinal`, `Format`.
- **[DateTimeExtensions](src/Boutquin.Domain/doc/DateTimeExtensions.md)** — `ConvertTimeZone` for converting between time zones.
- **[EnumExtensions](src/Boutquin.Domain/doc/EnumExtensions.md)** — `GetDescription` for retrieving `[Description]` attribute values.
- **[ResultExtensions](src/Boutquin.Domain/doc/ResultExtensions.md)** — `Match` methods for functional-style pattern matching on `Result` types.
- **[JsonElementExtensions](src/Boutquin.Domain/doc/JsonElementExtensions.md)** — `ToObject<T>` for deserializing `JsonElement` to typed objects.
- **[DecimalArrayExtensions](src/Boutquin.Domain/doc/DecimalArrayExtensions.md)** — `Variance` and `StandardDeviation` for decimal arrays.

### Converters

- **[DateOnlyConverter / DateOnlyDictionaryConverterFactory](src/Boutquin.Domain/doc/JsonConverters.md)** — ISO 8601 `DateOnly` serialization and DateOnly-keyed dictionaries with explicit null-value rejection on read.

### Value objects

- **[DateRange](src/Boutquin.Domain/doc/DateRange.md)** — Inclusive `DateOnly` ranges with containment, intersection, subtraction, and union operations.
- **[Money and Currency](src/Boutquin.Domain/doc/Money.md)** — Currency-safe monetary amounts backed by ISO 4217 currency codes.

### Exceptions

- **[DomainExceptions](src/Boutquin.Domain/doc/DomainExceptions.md)** — Self-describing exception hierarchy mapping to HTTP status codes (400–503), plus non-HTTP exceptions for data validation (`EmptyOrNullArrayException`, `InsufficientDataException`, etc.).

## Boutquin.AspNetCore Project

- **[CustomExceptionHandlerMiddleware](src/Boutquin.AspNetCore/doc/CustomExceptionHandlerMiddleware.md)** — Catches unhandled exceptions and produces RFC 7807 ProblemDetails JSON responses with `application/problem+json` content type.
- **[ModuleExtensions](src/Boutquin.AspNetCore/doc/ModuleExtensions.md)** — Module discovery, registration, and endpoint mapping for modular monolith applications. Includes an injectable assembly resolver for testability.
- **IModule** — Interface for self-registering application modules (`RegisterModule` + `MapEndpoints`).

## Boutquin.Validation Project

- **[ValidationException](src/Boutquin.Validation/doc/ValidationException.md)** — Exception wrapping FluentValidation failures with structured error data. Integrates with the middleware to produce grouped error responses.

## Quick Start

```csharp
// 1. Define a domain entity with a strongly typed ID:
public record OrderId(Guid Value) : StronglyTypedId<Guid>(Value);

public sealed class Order : Entity<OrderId>
{
    public Order(OrderId id) : base(id) { }

    public void Place()
    {
        // Business logic...
        RaiseDomainEvent(new OrderPlacedEvent(Id));
    }
}

// 2. Use the Result pattern for operations that can fail:
public Result<Order> GetOrder(OrderId id)
{
    var order = repository.Find(id);
    return order is not null
        ? Result.Success(order)
        : Result.Failure<Order>(new Error("Order.NotFound", "Order not found"));
}

// 3. Register modules before building the application, then map their endpoints:
builder.Services.RegisterModules();
var app = builder.Build();
app.UseCustomExceptionHandler();
app.MapEndpoints();

// 4. Throw domain exceptions — middleware handles the rest:
throw new NotFoundException("Order 42 was not found.");
// -> 404 ProblemDetails JSON response
```

## Architecture

See [ARCHITECTURE.md](ARCHITECTURE.md) for how the components fit together — layers, interface hierarchy, data flow, and component navigation.

## Package documentation

Each NuGet package contains a short, package-specific README so NuGet users see only the APIs they install. The repository README remains the family overview; the component guides under `src/*/doc/` provide the detailed API notes when browsing the source repository.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on reporting bugs, suggesting enhancements, and submitting pull requests. This project adheres to the Contributor Covenant [Code of Conduct](CODE_OF_CONDUCT.md).

## License

This project is licensed under the Apache 2.0 License. See the [LICENSE file](LICENSE.txt) for more information.
