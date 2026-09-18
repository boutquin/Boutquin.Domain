# Class Name: JsonElementExtensions

**Namespace:** `Boutquin.Domain.Extensions`

Provides extension methods for `System.Text.Json.JsonElement`.

## Methods

### `T? ToObject<T>(this JsonElement element, JsonSerializerOptions? options = null)`

Deserializes a `JsonElement` to a strongly-typed object. This is useful when working with `JsonDocument` or `JsonElement` (e.g., from a parsed JSON tree) and you need to convert a subtree to a specific type.

**Type Parameters:**
- `T` — The target deserialization type.

**Parameters:**
- `element` (JsonElement): The JSON element to deserialize.
- `options` (JsonSerializerOptions?, optional): Custom serializer options. If not provided, uses the defaults.

**Returns:** An instance of `T`, or `null` if the element represents a JSON null.

**How it works:** The method writes the `JsonElement` to a buffer using `ArrayBufferWriter<byte>` and `Utf8JsonWriter`, then deserializes from the buffer. This round-trip is necessary because `JsonElement` does not expose a direct deserialization API.

```csharp
using var doc = JsonDocument.Parse(jsonString);
var element = doc.RootElement.GetProperty("address");
var address = element.ToObject<Address>();
```
