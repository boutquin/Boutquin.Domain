# Class Name: Guard

The Guard class is a utility class that provides static methods to validate the preconditions for parameters in a method. It helps to ensure that input values are valid and appropriate for the given context.

## Methods

### `Against(bool condition)`

Checks if the given condition is true and throws the specified exception if it is.

#### Parameters

- `condition` (bool): The condition to check.

#### Returns

- An instance of the [GuardCondition class](https://github.com/boutquin/Boutquin.Domain/blob/master/Domain/doc/GuardCondition.md) to chain with With&lt;TException&gt; method.

### `AgainstNull<T>(Func<T> valueExpression) where T : class`

Checks if the given reference type value is null and throws an ArgumentNullException if it is. This overload accepts a Func&lt;T&gt; that returns the value and extracts its name using nameof.

#### Type Parameters

- `T` (class): The type of the value being checked. Must be a reference type.

#### Parameters

- `valueExpression` (Func&lt;T&gt;): A Func&lt;T&gt; that returns the value to check for null.

#### Exceptions

- `ArgumentNullException`: Thrown when the given value is null.

### `AgainstNullOrEmpty(Func<string> valueExpression)`

Checks if the given string value is null or empty and throws an ArgumentNullException if it is. This overload accepts a Func&lt;string&gt; that returns the value and extracts its name using nameof.

#### Parameters

- `valueExpression` (Func&lt;string&gt;): A Func&lt;string&gt; that returns the value to check for null or empty.

#### Exceptions

- `ArgumentNullException`: Thrown when the given string value is null or empty.

### `AgainstNullOrWhiteSpace(Func<string> valueExpression)`

Checks if the given string value is null, empty, or contains only whitespace characters and throws an ArgumentNullException if it is. This overload accepts a Func&lt;string&gt; that returns the value and extracts its name using nameof.

#### Parameters

- `valueExpression` (Func&lt;string&gt;): A Func&lt;string&gt; that returns the value to check for null, empty, or whitespace.

#### Exceptions

- `ArgumentNullException`: Thrown when the given string value is null, empty, or contains only whitespace characters.

### `AgainstOverflow(Func<string> valueExpression, int maxLength)`

Ensures that the length of the string returned by the given Func&lt;T&gt; expression is less than or equal to the specified maxLength.

#### Parameters

- `valueExpression` (Func&lt;string&gt;): A Func&lt;string&gt; that returns the string value to be checked.
- `maxLength` (int): The maximum length allowed for the string value. Must be greater than zero.

#### Exceptions

- `ArgumentOutOfRangeException`: Thrown when the length of the string value exceeds the specified maxLength or when maxLength is less than or equal to zero.
- `InvalidOperationException`: Thrown when the parameter name cannot be extracted from the expression.

### `AgainstNullOrEmptyAndOverflow(Func<string> valueExpression, int maxLength)`

Checks if the string returned by the given Func&lt;T&gt; expression is null, empty or exceeds the specified maximum length and throws an ArgumentException if it does.

#### Parameters

- `valueExpression` (Func&lt;string&gt;): A Func&lt;string&gt; that returns the string to check for null, empty, or exceeding the maximum length.
- `maxLength` (int): The maximum length allowed for the string.

#### Exceptions

- `ArgumentException`: Thrown when the given string is null, empty, or exceeds the maximum length.

### `AgainstNullOrWhiteSpaceAndOverflow(Func<string> valueExpression, int maxLength)`

Checks if the string returned by the given Func&lt;T&gt; expression is null, empty, consists only of white-space characters, or exceeds the specified maximum length and throws an ArgumentException if it does.

#### Parameters

- `valueExpression` (Func&lt;string&gt;): A Func&lt;string&gt; that returns the string to check for null, empty, white-space characters, or exceeding the maximum length.
- `maxLength` (int): The maximum length allowed for the string.

#### Exceptions

- `ArgumentException`: Thrown when the given string is null, empty, consists only of white-space characters, or exceeds the maximum length.
- `InvalidOperationException`: Thrown when the parameter name cannot be extracted from the expression.

### `AgainstNegative<T>(Func<T> valueExpression) where T : IComparable<T>`

Checks if the value returned by the given Func&lt;T&gt; expression is negative and throws an ArgumentOutOfRangeException if it is.

#### Type Parameters

- `T` (IComparable&lt;T&gt;): The type of the value. Must be a comparable value type.

#### Parameters

- `valueExpression` (Func&lt;T&gt;): A Func&lt;T&gt; that returns the value to check for negativity.

#### Exceptions

- `ArgumentOutOfRangeException`: Thrown when the given value is negative.
- `InvalidOperationException`: Thrown when the parameter name cannot be extracted from the expression.

### `AgainstNegativeOrZero<T>(Func<T> valueExpression) where T : IComparable<T>`

Ensures that the value returned by the given Func&lt;T&gt; expression is greater than zero.

#### Type Parameters

- `T` (IComparable&lt;T&gt;): The type of the value. Must be a comparable type.

#### Parameters

- `valueExpression` (Func&lt;T&gt;): A Func&lt;T&gt; that returns the value to be checked.

#### Exceptions

- `ArgumentOutOfRangeException`: Thrown when the value is negative or zero.
- `InvalidOperationException`: Thrown when the parameter name cannot be extracted from the expression.

### `AgainstUndefinedEnumValue<T>(Func<T> valueExpression) where T : Enum`

Checks if the enum value returned by the given Func&lt;T&gt; expression is defined and throws an ArgumentOutOfRangeException if it is not.

#### Type Parameters

- `T` (Enum): The type of the enum value.

#### Parameters

- `valueExpression` (Func&lt;T&gt;): A Func&lt;T&gt; that returns the enum value to check if it is defined.

#### Exceptions

- `ArgumentOutOfRangeException`: Thrown when the given enum value is not defined.
- `InvalidOperationException`: Thrown when the parameter name cannot be extracted from the expression.

### `AgainstOutOfRange<T>(Func<T> valueExpression, T min, T max) where T : IComparable<T>`

Checks if the value returned by the given Func&lt;T&gt; expression is within the specified range, inclusive.

#### Type Parameters

- `T` (IComparable&lt;T&gt;): The type of the value. Must be a comparable value type.

#### Parameters

- `valueExpression` (Func&lt;T&gt;): A Func&lt;T&gt; that returns the value to check for range.
- `min` (T): The minimum valid value, inclusive.
- `max` (T): The maximum valid value, inclusive.

#### Exceptions

- `ArgumentOutOfRangeException`: Thrown when the given value is not within the specified range.
- `InvalidOperationException`: Thrown when the parameter name cannot be extracted from the expression.

