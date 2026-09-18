# Class Name: Guard.GuardCondition

**Namespace:** `Boutquin.Domain.Helpers`

`GuardCondition` is the public nested type returned by `Guard.Against`. It provides the fluent API for conditional exception throwing.

## Constructor

### `GuardCondition(bool condition)`

Initializes a new instance of the GuardCondition class. The constructor is `internal`; consumers obtain instances from `Guard.Against()`.

**Parameters:**
- `condition` (bool): The condition to check.

## Methods

### `With<TException>() where TException : Exception, new()`

Throws the specified exception if the condition is true.

**Type Parameters:**
- `TException` (Exception): The type of exception to throw. Must have a parameterless constructor.

**Exceptions:** `TException` when the condition is true.

```csharp
Guard.Against(quantity <= 0).With<ArgumentException>();
```

### `With<TException>(string exceptionMessage) where TException : Exception`

Throws the specified exception with the provided message if the condition is true.

**Type Parameters:**
- `TException` (Exception): The type of exception to throw.

**Parameters:**
- `exceptionMessage` (string): The message for the exception.

**Exceptions:**
- `TException` when the condition is true.
- `ArgumentException` when exceptionMessage is null, empty, or whitespace.
- `InvalidOperationException` when the exception type doesn't have a string constructor.

```csharp
Guard.Against(quantity <= 0).With<ArgumentException>("Quantity must be greater than zero.");
```

### `With<TException>(string exceptionMessage, params object[] args) where TException : Exception`

Formats the provided message with invariant culture and throws the specified exception if the condition is true.

**Type Parameters:**
- `TException` (Exception): The type of exception to throw.

**Parameters:**
- `exceptionMessage` (string): The message for the exception.
- `args` (object[]): Values used to format the message.

**Exceptions:**
- `TException` when the condition is true.
- `ArgumentException` when exceptionMessage is null, empty, or whitespace.
- `FormatException` when the format string or arguments are invalid (only when the condition is true).
- `InvalidOperationException` when the exception type doesn't have a string constructor.

```csharp
Guard.Against(quantity <= 0).With<ArgumentException>("Quantity {0} must be positive.", quantity);
```

### `WithArgs<TException>(params object[] args) where TException : Exception`

Constructs and throws `TException` with the supplied constructor arguments when the condition is true.
Use this method for a constructor argument list rather than a formatted message; it is the
unambiguous replacement for the former params-only `With` call pattern.
