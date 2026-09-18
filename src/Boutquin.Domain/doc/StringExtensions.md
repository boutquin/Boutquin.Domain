# Class Name: StringExtensions

**Namespace:** `Boutquin.Domain.Extensions`

A static class providing extension methods for the `string` class.

## Methods

### `bool IsNullOrEmpty(this string? value)`

Determines whether the specified string is null or empty. Wraps `string.IsNullOrEmpty`.

```csharp
string name = null;
bool result = name.IsNullOrEmpty(); // true
```

### `bool IsNullOrWhiteSpace(this string? value)`

Determines whether the specified string is null, empty, or consists only of whitespace. Wraps `string.IsNullOrWhiteSpace`.

```csharp
string name = "   ";
bool result = name.IsNullOrWhiteSpace(); // true
```

### `string ToUpperCaseFirst(this string value)`

Converts the first Unicode scalar to uppercase using `CultureInfo.InvariantCulture`. An overload accepts an explicit `CultureInfo`.

**Exceptions:** `ArgumentNullException` when the input is null.

```csharp
"hello".ToUpperCaseFirst(); // "Hello"
"".ToUpperCaseFirst();      // ""
```

### `string ToLowerCaseFirst(this string value)`

Converts the first Unicode scalar to lowercase using `CultureInfo.InvariantCulture`. An overload accepts an explicit `CultureInfo`.

**Exceptions:** `ArgumentNullException` when the input is null.

```csharp
"Hello".ToLowerCaseFirst(); // "hello"
```

### `int Compare(this string? value, string? strB, StringComparison comparisonType)`

Compares two strings using the specified comparison options. Wraps `string.Compare`.

**Returns:** A signed integer indicating the relative order.

```csharp
"abc".Compare("ABC", StringComparison.OrdinalIgnoreCase); // 0
```

### `int CompareOrdinal(this string? value, string? strB)`

Compares two strings using ordinal (binary) sort rules. Wraps `string.CompareOrdinal`.

```csharp
"abc".CompareOrdinal("ABC"); // non-zero (case-sensitive)
```

### `string Format(this string format, params object[] args)`

Replaces format items in the string with string representations of corresponding objects, using the current culture. Wraps `string.Format` with `CultureInfo.CurrentCulture`.

```csharp
"Hello, {0}!".Format("World"); // "Hello, World!"
```

### `string Format(this string format, IFormatProvider provider, params object[] args)`

Replaces format items using the supplied provider, such as `CultureInfo.InvariantCulture` for stable output.
