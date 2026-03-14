# Class Name: GuardCondition

**Namespace:** `Boutquin.Domain.Helpers`

The GuardCondition class is a helper class used to chain the `Guard.Against` method with the `With<TException>` method, providing a fluent API for conditional exception throwing.

## Constructor

### `GuardCondition(bool condition)`

Initializes a new instance of the GuardCondition class. The constructor is marked as `internal` to prevent direct instantiation — instances are only created by `Guard.Against()`.

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

### `With<TException>(string exceptionMessage, Exception innerException) where TException : Exception`

Throws the specified exception with the provided message and inner exception if the condition is true.

**Type Parameters:**
- `TException` (Exception): The type of exception to throw.

**Parameters:**
- `exceptionMessage` (string): The message for the exception.
- `innerException` (Exception): The inner exception to include.

**Exceptions:**
- `TException` when the condition is true.
- `ArgumentException` when exceptionMessage is null, empty, or whitespace.
- `ArgumentNullException` when innerException is null.
- `InvalidOperationException` when the exception type doesn't have a (string, Exception) constructor.

```csharp
try { /* ... */ }
catch (Exception ex)
{
    Guard.Against(quantity <= 0).With<ArgumentException>("Quantity must be positive.", ex);
}
```
