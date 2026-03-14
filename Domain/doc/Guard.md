# Class Name: Guard

**Namespace:** `Boutquin.Domain.Helpers`

The Guard class is a static utility class that provides methods to validate preconditions for method parameters. It helps ensure that input values are valid and appropriate for the given context, following the [Guard Clause](https://refactoring.com/catalog/replaceNestedConditionalWithGuardClauses.html) pattern.

## Design

The Guard class provides two families of overloads for most validation methods:

1. **Expression-based overloads** — Accept a `Expression<Func<T>>` lambda (e.g., `Guard.AgainstNull(() => value)`). These compile the expression tree at runtime to extract both the value and the parameter name automatically. They offer the best developer ergonomics but have measurable overhead from expression compilation on every call.

2. **CallerArgumentExpression overloads** — Accept the value directly with `[CallerArgumentExpression]` (e.g., `Guard.AgainstNull(value)`). These use a C# 10 compiler feature to capture the parameter name at compile time with zero runtime overhead. Prefer these for performance-sensitive hot paths.

## Methods

### Fluent Condition API

#### `Against(bool condition)`

Checks if the given condition is true and returns a [GuardCondition](GuardCondition.md) to chain with `With<TException>()`.

**Parameters:**
- `condition` (bool): The condition to check.

**Returns:** An instance of the [GuardCondition class](GuardCondition.md) to chain with `With<TException>()`.

### Null / Default Checks

#### `AgainstNull<T>(Expression<Func<T>> valueExpression) where T : class`

Checks if the given reference type value is null and throws `ArgumentNullException` if it is. Extracts the parameter name from the expression tree.

**Parameters:**
- `valueExpression` (Expression&lt;Func&lt;T&gt;&gt;): An expression that returns the value to check for null.

**Exceptions:** `ArgumentNullException` when the value is null.

#### `AgainstNull<T>(T? value, [CallerArgumentExpression] string paramName = "") where T : class`

CallerArgumentExpression overload — checks if the reference type value is null. The parameter name is captured at compile time.

**Parameters:**
- `value` (T?): The value to check for null.
- `paramName` (string): Auto-captured by the compiler.

**Exceptions:** `ArgumentNullException` when the value is null.

#### `AgainstDefault<T>(Expression<Func<T>> valueExpression) where T : struct`

Checks if the given value type equals its default value (e.g., `0` for `int`, `Guid.Empty` for `Guid`).

**Parameters:**
- `valueExpression` (Expression&lt;Func&lt;T&gt;&gt;): An expression that returns the value to check.

**Exceptions:** `ArgumentException` when the value equals `default(T)`.

#### `AgainstDefault<T>(T value, [CallerArgumentExpression] string paramName = "") where T : struct`

CallerArgumentExpression overload for default value checks.

#### `AgainstNullOrDefault<T>(Expression<Func<T>> valueExpression)`

Checks if the given value is null (for reference types) or default (for value types).

### Collection Checks

#### `AgainstEmptyOrNullEnumerable<T>(Expression<Func<IEnumerable<T>>> enumerableExpression)`

Checks if the given enumerable is null or empty.

**Exceptions:** `ArgumentNullException` when null; `EmptyOrNullCollectionException` when empty.

#### `AgainstNullOrEmptyArray<T>(Expression<Func<T[]>> valueExpression)`

Checks if the given array is null or has zero elements.

**Exceptions:** `ArgumentNullException` when null; `EmptyOrNullArrayException` when empty.

#### `AgainstEmptyOrNullCollection<T>(Expression<Func<ICollection<T>>> collectionExpression)`

Checks if the given collection is null or has zero elements.

**Exceptions:** `ArgumentNullException` when null; `EmptyOrNullCollectionException` when empty.

#### `AgainstEmptyOrNullDictionary<TKey, TValue>(Expression<Func<IDictionary<TKey, TValue>>> dictionaryExpression)`

Checks if the given dictionary is null or has zero entries.

**Exceptions:** `ArgumentNullException` when null; `EmptyOrNullDictionaryException` when empty.

#### `AgainstEmptyOrNullReadOnlyDictionary<TKey, TValue>(Expression<Func<IReadOnlyDictionary<TKey, TValue>>> dictionaryExpression)`

Same as above but for `IReadOnlyDictionary<TKey, TValue>`.

### String Checks

#### `AgainstNullOrEmpty(Expression<Func<string>> valueExpression)`

Checks if the given string is null or empty.

**Exceptions:** `ArgumentNullException` when null or empty.

#### `AgainstNullOrEmpty(string? value, [CallerArgumentExpression] string paramName = "")`

CallerArgumentExpression overload for null/empty string checks.

#### `AgainstNullOrWhiteSpace(Expression<Func<string>> valueExpression)`

Checks if the given string is null, empty, or consists only of whitespace.

**Exceptions:** `ArgumentNullException` when null, empty, or whitespace.

#### `AgainstNullOrWhiteSpace(string? value, [CallerArgumentExpression] string paramName = "")`

CallerArgumentExpression overload for null/whitespace string checks.

#### `AgainstOverflow(Expression<Func<string>> valueExpression, int maxLength)`

Ensures the string length does not exceed the specified maximum.

**Parameters:**
- `valueExpression` (Expression&lt;Func&lt;string&gt;&gt;): An expression returning the string to check.
- `maxLength` (int): The maximum allowed length (must be > 0).

**Exceptions:** `ArgumentOutOfRangeException` when the string exceeds maxLength.

#### `AgainstNullOrEmptyAndOverflow(Expression<Func<string>> valueExpression, int maxLength)`

Combined check: null/empty + overflow.

#### `AgainstNullOrWhiteSpaceAndOverflow(Expression<Func<string>> valueExpression, int maxLength)`

Combined check: null/whitespace + overflow.

### Numeric Checks

#### `AgainstNegative<T>(Expression<Func<T>> valueExpression) where T : struct, IComparable<T>`

Checks if the value is negative (less than zero).

**Exceptions:** `ArgumentOutOfRangeException` when negative.

#### `AgainstNegative<T>(T value, [CallerArgumentExpression] string paramName = "") where T : struct, IComparable<T>`

CallerArgumentExpression overload for negativity checks.

#### `AgainstNegativeOrZero<T>(Expression<Func<T>> valueExpression) where T : struct, IComparable<T>`

Checks if the value is zero or negative.

**Exceptions:** `ArgumentOutOfRangeException` when negative or zero.

#### `AgainstNegativeOrZero<T>(T value, [CallerArgumentExpression] string paramName = "") where T : struct, IComparable<T>`

CallerArgumentExpression overload.

#### `AgainstOutOfRange<T>(Expression<Func<T>> valueExpression, T min, T max) where T : struct, IComparable<T>`

Checks if the value is within the specified range [min, max], inclusive.

**Parameters:**
- `valueExpression`: Expression returning the value.
- `min` (T): Minimum valid value (inclusive).
- `max` (T): Maximum valid value (inclusive).

**Exceptions:** `ArgumentOutOfRangeException` when outside range.

#### `AgainstOutOfRange<T>(T value, T min, T max, [CallerArgumentExpression] string paramName = "") where T : struct, IComparable<T>`

CallerArgumentExpression overload for range checks.

### Enum Checks

#### `AgainstNonEnumType<T>()`

Checks that the type parameter `T` is an enum type.

**Exceptions:** `ArgumentException` when `T` is not an enum.

#### `AgainstUndefinedEnumValue<T>(Expression<Func<T>> valueExpression) where T : Enum`

Checks if the enum value is defined in the enum type.

**Exceptions:** `ArgumentOutOfRangeException` when the value is not a defined member.

## Example Usage

```csharp
// Expression-based (automatic parameter name extraction):
Guard.AgainstNull(() => customer);
Guard.AgainstNullOrWhiteSpace(() => name);
Guard.AgainstOutOfRange(() => age, 0, 150);

// CallerArgumentExpression-based (zero overhead, C# 10+):
Guard.AgainstNull(customer);
Guard.AgainstNullOrWhiteSpace(name);
Guard.AgainstOutOfRange(age, 0, 150);

// Fluent condition API:
Guard.Against(quantity <= 0).With<ArgumentException>("Quantity must be positive.");
```
