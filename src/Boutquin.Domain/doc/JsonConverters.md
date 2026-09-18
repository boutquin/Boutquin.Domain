# JSON Converters for DateOnly

**Namespace:** `Boutquin.Domain.Converters`

Custom `System.Text.Json` converters for ISO 8601 `DateOnly` values and DateOnly-keyed dictionaries.
.NET 10 already supports these types; the converters add explicit format and validation policies.

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

Serializes `Dictionary<DateOnly, TValue>` and `SortedDictionary<DateOnly, TValue>` with ISO 8601 keys and rejects null values on deserialization.

**Why use it:** .NET 10 supports `DateOnly` dictionary keys without this factory. Use the factory when deserialization must reject null values while preserving the requested concrete dictionary type.

Registering the factory alone is sufficient. A separate `DateOnlyConverter` registration is optional.
Keys are read through the registered DateOnly converter's `ReadAsPropertyName` method, rather than
its scalar `Read` method. Written keys always use `yyyy-MM-dd`; a custom property-name reader must
accept this format to round-trip the factory's output.

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
