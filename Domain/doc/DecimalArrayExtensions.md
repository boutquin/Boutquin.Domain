# Class Name: DecimalArrayExtensions

The DecimalArrayExtensions class is a static class that provides extension methods for working with arrays of decimal values. It includes methods for calculating the average, variance, and standard deviation of an array of decimal values.

## Properties

- `CalculationType` (enum): an enum that represents the type of calculation for variance and standard deviation. It has two values: Sample and Population.

## Methods


### `Variance(this decimal[] values, CalculationType calculationType = CalculationType.Sample) -> decimal`

Calculates the variance of an array of decimal values.

Throws an `EmptyOrNullArrayException` if the input array is empty, and an `InsufficientDataException` if the input array contains less than two elements for sample calculation.

#### Parameters

- `values` (decimal[]): The array of decimal values.
- `calculationType` (CalculationType, optional): The type of calculation (sample or population). Defaults to CalculationType.Sample.

#### Returns

(decimal): The variance of the values.

#### Example Usage

```csharp
decimal[] values = { 1.5m, 2.0m, 3.5m, 4.2m, 5.8m };
decimal variance = values.Variance();
```


### `StandardDeviation(this decimal[] values, CalculationType calculationType = CalculationType.Sample) -> decimal`

Calculates the standard deviation of an array of decimal values.

Throws an `EmptyOrNullArrayException` if the input array is empty, and an `InsufficientDataException` if the input array contains less than two elements for sample calculation.

#### Parameters

- `values` (decimal[]): The array of decimal values.
- `calculationType` (CalculationType, optional): The type of calculation (sample or population). Defaults to CalculationType.Sample.

#### Returns

(decimal): The standard deviation of the values.

#### Example Usage

```csharp
decimal[] values = { 1.5m, 2.0m, 3.5m, 4.2m, 5.8m };
decimal std_deviation = values.StandardDeviation();
```


## Exceptions

### `EmptyOrNullArrayException`

Custom exception for an empty or null input array.

#### Constructor Parameters

- `message` (string): The exception message.

#### Example Usage

```csharp
throw new EmptyOrNullArrayException("Input array is null.");
```


### `InsufficientDataException`

Custom exception for insufficient data for a sample calculation.

#### Constructor Parameters

- `message` (string): The exception message.

#### Example Usage

```csharp
throw new InsufficientDataException("Not enough data for sample calculation.");
```


## Inner Classes

### `ExceptionMessages`

Contains constants for exception messages.

#### Fields

- `EmptyOrNullArray` (string): Input array must not be empty or null.
- `InsufficientDataForSampleCalculation` (string): Input array must have at least two elements for sample calculation.

#### Example Usage

```csharp
public decimal Variance(this decimal[] values, CalculationType calculationType = CalculationType.Sample)
{
    if (values == null || values.Length == 0)
    {
        throw new EmptyOrNullArrayException(ExceptionMessages.EmptyOrNullArray);
    }

    if (calculationType == CalculationType.Sample && values.Length == 1)
    {
        throw new InsufficientDataException(ExceptionMessages.InsufficientDataForSampleCalculation);
    }

    var avg = values.Average();
    var sumOfSquares = values.Sum(x => (x - avg) * (x - avg));
    var denominator = calculationType == CalculationType.Sample ? values.Length - 1 : values.Length;
    return sumOfSquares / denominator;
}
```
