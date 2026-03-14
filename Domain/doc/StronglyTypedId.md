# Record Name: StronglyTypedId&lt;TValue&gt;

**Namespace:** `Boutquin.Domain.Helpers`

An abstract record providing a base for creating strongly typed identifiers. Wrapping primitive types (e.g., `Guid`, `int`) in domain-specific types prevents "primitive obsession" — where two `Guid` parameters can be accidentally swapped because the type system treats them identically.

## Design

- **Record type** — Provides value-based equality out of the box. Two `UserId(Guid.Parse("..."))` instances with the same GUID are automatically equal.
- **`where TValue : notnull`** — Ensures identifiers cannot be null.
- **Sealed `ToString()`** — Returns the raw value string, not the record's default format. This produces clean output for logging and serialization.

## Properties

### `TValue Value { get; init; }`

The underlying value of the identifier.

## Methods

### `sealed override string ToString()`

Returns the string representation of `Value` only (not the record name or braces).

## Example

```csharp
// Define strongly typed IDs for domain entities:
public record UserId(Guid Value) : StronglyTypedId<Guid>(Value);
public record OrderId(int Value) : StronglyTypedId<int>(Value);

// Usage — the compiler prevents mixing them up:
public Order GetOrder(OrderId orderId) { /* ... */ }
public User GetUser(UserId userId) { /* ... */ }

var userId = new UserId(Guid.NewGuid());
var orderId = new OrderId(42);

// GetOrder(userId);  // Compile error — prevents accidental parameter swap
Console.WriteLine(userId); // Outputs: {guid} (not "UserId { Value = {guid} }")
```
