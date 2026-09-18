# Class Name: DateTimeExtensions

**Namespace:** `Boutquin.Domain.Extensions`

A static class providing extension methods for `DateTime` time zone conversions.

## Methods

### `DateTime ConvertTimeZone(this DateTime dateTime, string sourceTimeZoneId, string targetTimeZoneId)`

Converts a `DateTime` from one time zone to another by first converting to UTC, then to the target time zone.

**Parameters:**
- `dateTime` (DateTime): The date/time to convert.
- `sourceTimeZoneId` (string): The IANA or Windows time zone ID of the source (e.g., `"UTC"`, `"America/New_York"`).
- `targetTimeZoneId` (string): The IANA or Windows time zone ID of the target.

**Returns:** The `DateTime` expressed in the target time zone.

**Exceptions:**
- `ArgumentNullException` when either time zone ID is null.
- `TimeZoneNotFoundException` when either time zone ID is not recognized.

```csharp
var utcNow = DateTime.UtcNow;
var eastern = utcNow.ConvertTimeZone("UTC", "America/New_York");
var pacific = eastern.ConvertTimeZone("America/New_York", "America/Los_Angeles");
```
