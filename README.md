# Boutquin.Domain

![Nuget](https://img.shields.io/nuget/vpre/boutquin.domain?style=for-the-badge) ![License](https://img.shields.io/github/license/boutquin/boutquin.domain?style=for-the-badge)

A .NET domain layer library providing DDD building blocks, guard clauses, result types, JSON converters, and ASP.NET Core middleware for modular monolith applications.

## Solution Structure

The solution contains three library projects and a test project:

| Project | NuGet Package | Description |
|---------|---------------|-------------|
| **Domain** (`Boutquin.Domain`) | boutquin.domain | Core domain abstractions, helpers, extensions, converters, and exceptions |
| **Boutquin.AspNetCore** | — | ASP.NET Core middleware, module system, and exception handling |
| **Boutquin.Validation** | — | FluentValidation integration and validation exception handling |
| **UnitTests** (`Boutquin.UnitTests`) | — | xUnit tests with FluentAssertions (308 tests, ~91% line coverage) |

## Domain Project

### Abstractions

The building blocks for Domain-Driven Design:

- **[Entity&lt;TEntityId&gt;](Domain/doc/Entity.md)** — Abstract base class with identity-based equality, domain event buffering, and ORM-compatible constructors.
- **[Result / Result&lt;TValue&gt;](Domain/doc/Result.md)** — Functional error handling — return success/failure instead of throwing exceptions.
- **[Error](Domain/doc/Result.md)** — Immutable record representing a domain error with code and name.
- **IEntity** — Interface for entities that generate domain events.
- **IDomainEvent** — Marker interface extending MediatR's `INotification` for domain events.
- **IUnitOfWork** — Defines the persistence boundary (`SaveChangesAsync`).

### Helpers

- **[Guard](Domain/doc/Guard.md)** — Static utility class for parameter validation with two API styles: expression-based (auto-extracts parameter names) and `CallerArgumentExpression`-based (zero overhead).
- **[GuardCondition](Domain/doc/GuardCondition.md)** — Fluent chaining helper for `Guard.Against().With<TException>()`.
- **[StronglyTypedId&lt;TValue&gt;](Domain/doc/StronglyTypedId.md)** — Abstract record for wrapping primitives as domain-specific ID types, preventing primitive obsession.

### Extensions

- **[StringExtensions](Domain/doc/StringExtensions.md)** — `IsNullOrEmpty`, `IsNullOrWhiteSpace`, `ToUppercaseFirst`, `ToLowerCaseFirst`, `Compare`, `CompareOrdinal`, `Format`.
- **[DateTimeExtensions](Domain/doc/DateTimeExtensions.md)** — `ConvertTimeZone` for converting between time zones.
- **[EnumExtensions](Domain/doc/EnumExtensions.md)** — `GetDescription` for retrieving `[Description]` attribute values.
- **[ResultExtensions](Domain/doc/ResultExtensions.md)** — `Match` methods for functional-style pattern matching on `Result` types.
- **[JsonElementExtensions](Domain/doc/JsonElementExtensions.md)** — `ToObject<T>` for deserializing `JsonElement` to typed objects.
- **[DecimalArrayExtensions](Domain/doc/DecimalArrayExtensions.md)** — `Variance` and `StandardDeviation` for decimal arrays.

### Converters

- **[DateOnlyConverter / DateOnlyDictionaryConverterFactory](Domain/doc/JsonConverters.md)** — Custom `System.Text.Json` converters for `DateOnly` values and dictionaries with `DateOnly` keys (works around a `System.Text.Json` limitation).

### Exceptions

- **[DomainExceptions](Domain/doc/DomainExceptions.md)** — Self-describing exception hierarchy mapping to HTTP status codes (400–503), plus non-HTTP exceptions for data validation (`EmptyOrNullArrayException`, `InsufficientDataException`, etc.).

## Boutquin.AspNetCore Project

- **[CustomExceptionHandlerMiddleware](AspNetCore/doc/CustomExceptionHandlerMiddleware.md)** — Catches unhandled exceptions and produces RFC 7807 ProblemDetails JSON responses with `application/problem+json` content type.
- **[ModuleExtensions](AspNetCore/doc/ModuleExtensions.md)** — Module discovery, registration, and endpoint mapping for modular monolith applications. Includes an injectable assembly resolver for testability.
- **IModule** — Interface for self-registering application modules (`RegisterModule` + `MapEndpoints`).

## Boutquin.Validation Project

- **[ValidationException](Validation/doc/ValidationException.md)** — Exception wrapping FluentValidation failures with structured error data. Integrates with the middleware to produce grouped error responses.

## Quick Start

```csharp
// 1. Define a domain entity with strongly typed ID:
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

// 3. Register modules in Startup:
builder.Services.RegisterModules();
var app = builder.Build();
app.UseCustomExceptionHandler();
app.MapEndpoints();

// 4. Throw domain exceptions — middleware handles the rest:
throw new NotFoundException("Order 42 was not found.");
// -> 404 ProblemDetails JSON response
```

## Contributing

If you'd like to contribute, please feel free to submit a pull request or open an issue with your suggestions or improvements.

## License

This project is licensed under the Apache 2.0 License. See the [LICENSE file](LICENSE.txt) for more information.
