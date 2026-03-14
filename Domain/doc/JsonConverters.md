# JSON Converters for DateOnly

**Namespace:** `Boutquin.Domain.Converters`

Custom `System.Text.Json` converters for `DateOnly` — a type introduced in .NET 6 that lacks built-in serialization support in `System.Text.Json` for certain scenarios.

## DateOnlyConverter

### `sealed class DateOnlyConverter : JsonConverter<DateOnly>`

Serializes and deserializes `DateOnly` values as ISO 8601 strings (`yyyy-MM-dd`) using `CultureInfo.InvariantCulture`.

**Methods:**

| Method | Description |
|--------|-------------|
| `Read(ref Utf8JsonReader, Type, JsonSerializerOptions)` | Parses a JSON string to `DateOnly`. Throws `JsonException` if the string is null or not a valid date. |
| `Write(Utf8JsonWriter, DateOnly, JsonSerializerOptions)` | Writes the `DateOnly` as a `yyyy-MM-dd` string. |

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new DateOnlyConverter());

var json = JsonSerializer.Serialize(new DateOnly(2024, 3, 15), options);
// json: "2024-03-15"
```

## DateOnlyDictionaryConverterFactory

### `sealed class DateOnlyDictionaryConverterFactory : JsonConverterFactory`

Enables serialization of `Dictionary<DateOnly, TValue>` and `SortedDictionary<DateOnly, TValue>`.

**The problem:** `System.Text.Json` cannot serialize dictionaries with non-string keys that lack a built-in string conversion. `DateOnly` is one such type — serializing a `Dictionary<DateOnly, decimal>` without this converter throws `NotSupportedException`.

**The solution:** This factory inspects the dictionary's generic type arguments at runtime, constructs the appropriate typed converter via `Type.MakeGenericType`, and handles both `Dictionary` and `SortedDictionary` with separate inner converter classes (required because `System.Text.Json` matches converters by exact generic type).

**Methods:**

| Method | Description |
|--------|-------------|
| `CanConvert(Type)` | Returns `true` for `Dictionary<DateOnly, T>` and `SortedDictionary<DateOnly, T>`. |
| `CreateConverter(Type, JsonSerializerOptions)` | Creates the appropriate inner converter for the dictionary type. |

**Key design decisions:**
- **Two inner converters** — `SortedDictionary<,>` and `Dictionary<,>` have different generic type definitions; a single converter over `IDictionary` would lose the concrete type during deserialization.
- **ISO 8601 keys** — Dictionary keys are serialized as `yyyy-MM-dd` property names using `InvariantCulture`.
- **Null value rejection** — Deserialization throws `JsonException` for null values to prevent downstream `NullReferenceException`.

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new DateOnlyDictionaryConverterFactory());

var prices = new SortedDictionary<DateOnly, decimal>
{
    [new DateOnly(2024, 1, 1)] = 100.50m,
    [new DateOnly(2024, 1, 2)] = 101.25m,
};

var json = JsonSerializer.Serialize(prices, options);
// {"2024-01-01":100.50,"2024-01-02":101.25}

var deserialized = JsonSerializer.Deserialize<SortedDictionary<DateOnly, decimal>>(json, options);
```
