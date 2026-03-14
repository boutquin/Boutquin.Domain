# Class Name: ResultExtensions

**Namespace:** `Boutquin.Domain.Extensions`

Provides `Match` extension methods for the [Result](Result.md) type, enabling functional-style pattern matching on success/failure outcomes.

## Methods

### `T Match<T>(this Result result, Func<T> onSuccess, Func<Error, T> onFailure)`

Executes one of two functions based on the result's success or failure state.

**Type Parameters:**
- `T` — The return type of both functions.

**Parameters:**
- `result` (Result): The result to match against.
- `onSuccess` (Func&lt;T&gt;): Function to execute on success.
- `onFailure` (Func&lt;Error, T&gt;): Function to execute on failure, receiving the error.

**Returns:** The return value of whichever function was executed.

```csharp
var result = PerformOperation();
var message = result.Match(
    onSuccess: () => "Operation succeeded.",
    onFailure: error => $"Failed: {error.Name}");
```

### `TResult Match<TResult, TValue>(this Result<TValue> result, Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)`

Executes one of two functions based on the result's success or failure state, with access to the result's value on success.

**Type Parameters:**
- `TResult` — The return type of both functions.
- `TValue` — The type of value carried by the result.

**Parameters:**
- `result` (Result&lt;TValue&gt;): The result to match against.
- `onSuccess` (Func&lt;TValue, TResult&gt;): Function to execute on success, receiving the value.
- `onFailure` (Func&lt;Error, TResult&gt;): Function to execute on failure, receiving the error.

**Returns:** The return value of whichever function was executed.

```csharp
var result = Divide(10, 2);
var message = result.Match(
    onSuccess: value => $"Result: {value}",
    onFailure: error => $"Error: {error.Name}");
```
