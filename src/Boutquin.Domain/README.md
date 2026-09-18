# Boutquin.Domain

Domain-driven design building blocks for .NET 10: entities, results, guard clauses,
strongly typed IDs, value objects, domain exceptions, JSON converters, and small
utility extensions.

## Install

```bash
dotnet add package Boutquin.Domain
```

## Highlights

- `Entity<TEntityId>` provides identity-based equality for entities.
- `Result` and `Result<TValue>` represent expected failures without throwing. An
  `Error` contains `Code`, `Description`, and `ErrorType`.
- `Guard` offers both modern `CallerArgumentExpression` overloads and legacy
  expression-based overloads. Use `Guard.Against(...).WithArgs(...)` when an exception
  requires explicit constructor arguments.
- `StronglyTypedId<T>` gives identifier value objects invariant-culture string
  formatting.
- `DateRange`, `Money`, and ISO 4217 `Currency` model common domain values.

```csharp
using Boutquin.Domain.Abstractions;
using Boutquin.Domain.Enumerations;
using Boutquin.Domain.Extensions;

Result<string> result = "created";

var price = Currency.USD.Amount(19.99m);
var total = price + Currency.USD.Amount(5m);
```

`Money` equality is structural and never throws across currencies. Arithmetic and
ordering require matching currencies and throw `CurrencyMismatchException` when
currencies differ.

## Documentation

Component guides and API examples are maintained in the
[repository documentation](https://github.com/boutquin/Boutquin.Domain/tree/main/src/Boutquin.Domain/doc).
See the [repository README](https://github.com/boutquin/Boutquin.Domain#readme) for
the related ASP.NET Core and FluentValidation packages.

## License

Apache-2.0. See the [repository license](https://github.com/boutquin/Boutquin.Domain/blob/main/LICENSE.txt).
