# Class Name: Result / Result&lt;TValue&gt;

**Namespace:** `Boutquin.Domain.Abstractions`

The Result pattern provides a functional approach to error handling — methods return a `Result` or `Result<TValue>` instead of throwing exceptions for expected failures. This makes error handling explicit in the type system and eliminates the need for try/catch blocks in business logic.

## Error Record

### `record Error(string Code, string Name)`

An immutable record representing a domain error with a unique code and human-readable name.

**Properties:**
- `Code` (string): A unique identifier for the error type (e.g., `"Error.NullValue"`).
- `Name` (string): A human-readable description (e.g., `"Null value was provided"`).

**Static Fields:**
- `Error.None` — Represents no error (used internally for successful results).
- `Error.NullValue` — Represents a null value error (used by `Result.Create`).

## Result (non-generic)

Represents the outcome of an operation — success or failure — without a return value.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsSuccess` | `bool` | `true` if the operation succeeded. |
| `IsFailure` | `bool` | `true` if the operation failed (computed as `!IsSuccess`). |
| `Error` | `Error` | The error associated with the failure. Equals `Error.None` for successful results. |

### Constructor

#### `protected Result(bool isSuccess, Error error)`

Enforces invariants:
- A successful result **must not** have an error (throws `InvalidOperationException`).
- A failed result **must** have an error (throws `InvalidOperationException`).

### Static Factory Methods

#### `Result.Success()`

Creates a successful result with no value.

#### `Result.Failure(Error error)`

Creates a failed result with the specified error.

#### `Result.Success<TValue>(TValue value)`

Creates a successful result wrapping the specified value.

#### `Result.Failure<TValue>(Error error)`

Creates a failed result for a value-returning operation.

#### `Result.Create<TValue>(TValue? value)`

Creates a result based on nullability: non-null → success with value; null → failure with `Error.NullValue`.

## Result&lt;TValue&gt;

Extends `Result` for operations that return a value on success.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `TValue` | The operation's return value. Throws `InvalidOperationException` if accessed on a failed result. |

### Constructor

#### `internal Result(TValue? value, bool isSuccess, Error error)`

Internal — instances are created via the static factory methods on `Result`.

### Operators

#### `implicit operator Result<TValue>(TValue? value)`

Allows returning a value directly from a method with return type `Result<TValue>`. Delegates to `Result.Create`.

## Example Usage

```csharp
// Returning results from methods:
public Result<int> Divide(int a, int b)
{
    if (b == 0)
        return Result.Failure<int>(new Error("DivideByZero", "Cannot divide by zero"));
    return Result.Success(a / b);
}

// Consuming results:
var result = Divide(10, 2);
if (result.IsSuccess)
    Console.WriteLine($"Result: {result.Value}");
else
    Console.WriteLine($"Error: {result.Error.Name}");

// Using Match (see ResultExtensions):
var message = result.Match(
    onSuccess: value => $"Result: {value}",
    onFailure: error => $"Error: {error.Name}");

// Implicit conversion:
public Result<string> GetName() => "Alice"; // implicitly wraps in Success
```

## See Also

- [ResultExtensions](ResultExtensions.md) — `Match` methods for pattern matching on results.
