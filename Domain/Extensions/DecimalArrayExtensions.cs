// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Extensions;

using System.Diagnostics.CodeAnalysis;
using Exceptions;
using Helpers;

/// <summary>
/// Provides extension methods for calculating statistical measures on an array of decimal values.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class DecimalArrayExtensions
{
    /// <summary>
    /// Enum to represent the calculation type for variance and standard deviation.
    /// </summary>
    public enum CalculationType
    {
        Sample,
        Population
    }

    /// <summary>
    /// Calculates the variance of an array of decimal values.
    /// </summary>
    /// <param name="values">The array of decimal values.</param>
    /// <param name="calculationType">The type of calculation (sample or population).</param>
    /// <returns>The variance of the values.</returns>
    /// <exception cref="EmptyOrNullArrayException">Thrown when the input array is empty.</exception>
    /// <exception cref="InsufficientDataException">Thrown when the input array contains less than two elements for sample calculation.</exception>
    public static decimal Variance(this decimal[] values, CalculationType calculationType = CalculationType.Sample)
    {
        // Ensure the input array is not null or empty
        Guard.AgainstNullOrEmptyArray(() => values);

        // Ensure there is enough data for sample variance calculation
        Guard.Against(calculationType == CalculationType.Sample && values.Length == 1)
            .With<InsufficientDataException>(ExceptionMessages.InsufficientDataForSampleCalculation);

        // Calculate the average (mean) of the input values
        var avg = values.Average();

        // Calculate the sum of squared differences between each value and the average
        var sumOfSquares = values.Sum(x => (x - avg) * (x - avg));

        // Choose the denominator based on the calculation type (sample or population)
        var denominator = calculationType == CalculationType.Sample ? values.Length - 1 : values.Length;

        // Calculate and return the variance
        return sumOfSquares / denominator;
    }

    /// <summary>
    /// Calculates the standard deviation of an array of decimal values.
    /// </summary>
    /// <param name="values">The array of decimal values.</param>
    /// <param name="calculationType">The type of calculation (sample or population).</param>
    /// <returns>The standard deviation of the values.</returns>
    /// <exception cref="EmptyOrNullArrayException">Thrown when the input array is empty.</exception>
    /// <exception cref="InsufficientDataException">Thrown when the input array contains less than two elements for sample calculation.</exception>

    public static decimal StandardDeviation(this decimal[] values, CalculationType calculationType = CalculationType.Sample)
        => (decimal)Math.Sqrt((double)values.Variance(calculationType));
}
