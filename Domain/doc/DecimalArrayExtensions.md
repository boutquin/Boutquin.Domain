# Class Name: DecimalArrayExtensions

**Namespace:** `Boutquin.Domain.Extensions`

A static class that provides extension methods for working with arrays of decimal values. It includes methods for calculating the variance and standard deviation of an array of decimal values, supporting both sample and population calculations.

## Enums

### `CalculationType`

Specifies the type of statistical calculation.

| Value | Description |
|-------|-------------|
| `Sample` | Sample calculation — divides by `n - 1` (Bessel's correction). Default. |
| `Population` | Population calculation — divides by `n`. |

## Methods

### `Variance(this decimal[] values, CalculationType calculationType = CalculationType.Sample) -> decimal`

Calculates the variance of an array of decimal values.

**Parameters:**
- `values` (decimal[]): The array of decimal values.
- `calculationType` (CalculationType, optional): Sample or Population. Defaults to `Sample`.

**Returns:** (decimal) The variance of the values.

**Exceptions:**
- `EmptyOrNullArrayException` when the input array is null or empty.
- `InsufficientDataException` when the array contains fewer than two elements for sample calculation.

```csharp
decimal[] values = { 1.5m, 2.0m, 3.5m, 4.2m, 5.8m };
decimal variance = values.Variance(); // sample variance
decimal popVariance = values.Variance(CalculationType.Population);
```

### `StandardDeviation(this decimal[] values, CalculationType calculationType = CalculationType.Sample) -> decimal`

Calculates the standard deviation of an array of decimal values (the square root of the variance).

**Parameters:**
- `values` (decimal[]): The array of decimal values.
- `calculationType` (CalculationType, optional): Sample or Population. Defaults to `Sample`.

**Returns:** (decimal) The standard deviation of the values.

**Exceptions:**
- `EmptyOrNullArrayException` when the input array is null or empty.
- `InsufficientDataException` when the array contains fewer than two elements for sample calculation.

```csharp
decimal[] values = { 1.5m, 2.0m, 3.5m, 4.2m, 5.8m };
decimal stdDev = values.StandardDeviation();
```

## Related Types

### `ExceptionMessages`

Static class containing constants for exception messages used by the validation:
- `EmptyOrNullArray` — "Input array must not be empty or null."
- `InsufficientDataForSampleCalculation` — "Input array must have at least two elements for sample calculation."
