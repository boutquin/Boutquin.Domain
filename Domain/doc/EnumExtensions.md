# Class Name: EnumExtensions

**Namespace:** `Boutquin.Domain.Extensions`

A static class providing extension methods for enum types.

## Methods

### `string GetDescription<T>(this T enumValue) where T : Enum`

Retrieves the `[Description]` attribute value for an enum member. If no `DescriptionAttribute` is present, returns the enum member's name as a string.

**Type Parameters:**
- `T` — Must be an `Enum` type.

**Parameters:**
- `enumValue` (T): The enum value to get the description for.

**Returns:** The description string from the `DescriptionAttribute`, or the enum member name.

**Exceptions:** `ArgumentException` when the provided type is not an enum or the value is not a valid member.

```csharp
public enum Colors
{
    [Description("Bright Red Color")]
    Red,
    Blue,
    Green
}

Colors.Red.GetDescription();  // "Bright Red Color"
Colors.Blue.GetDescription(); // "Blue"
```
