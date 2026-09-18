# Money and Currency

**Namespaces:** `Boutquin.Domain.ValueObjects`, `Boutquin.Domain.Enumerations`, and `Boutquin.Domain.Extensions`

`Money` is an immutable `readonly record struct` pairing a decimal amount with an ISO
4217 `Currency`. It does not impose a minor-unit scale at construction; use its
`Round` overloads where a boundary needs quantization.

```csharp
var price = Currency.USD.Amount(19.99m);
var tax = price * 1.13m;
var total = tax + Currency.USD.Amount(5m);
var rounded = Money.Round(total, 2);
```

Equality is structural: `5 USD == 5 EUR` is `false`, never an exception. Arithmetic
between two `Money` values and all ordering operations require the same currency and
throw `CurrencyMismatchException` otherwise. Operations with a bare decimal preserve
the existing currency.

`Currency` members use ISO 4217 alphabetic identifiers and their ISO numeric code as
the enum value. `Currency.Unspecified` and undefined enum values are not valid
denominations: constructing `Money` or calling `Amount(...)` with either throws
`ArgumentOutOfRangeException`. Retrieve a member's human-readable name with
`GetDescription()`.

As with every struct, `default(Money)` can still exist and carries
`Currency.Unspecified`. Structural equality and hashing remain available for that
sentinel value, but currency-dependent operations and `ToString()` throw
`InvalidOperationException` until the value is replaced with a constructed `Money`.

`CurrencyMismatchException` derives directly from `Exception`; its currency
constructor exposes `Expected` and `Actual`.
