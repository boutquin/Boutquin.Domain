// Copyright (c) 2023 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Boutquin.Domain.Exceptions;

namespace Boutquin.Domain.Extensions;

/// <summary>
/// Provides extension methods for calculating statistical measures on an array of decimal values.
/// </summary>
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
    /// Contains constants for exception messages.
    /// </summary>
    public static class ExceptionMessages
    {
        public const string EmptyOrNullArray = "Input array must not be empty or null.";
        public const string InsufficientDataForSampleCalculation = "Input array must have at least two elements for sample calculation.";
    }

    /// <summary>
    /// Custom exception for invalid input data.
    /// </summary>
    public class InvalidInputDataException : Exception
    {
        public InvalidInputDataException(string message) : base(message) { }
    }

    /// <summary>
    /// Calculates the average of an array of decimal values.
    /// </summary>
    /// <param name="values">The array of decimal values.</param>
    /// <returns>The average of the values.</returns>
    /// <exception cref="InvalidInputDataException">Thrown when the input array is empty or null.</exception>
    public static decimal Average(this decimal[] values)
    {
        if (values == null || values.Length == 0)
        {
            throw new EmptyOrNullArrayException(ExceptionMessages.EmptyOrNullArray);
        }

        return values.Sum() / values.Length;
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

    /// <summary>
    /// Calculates the standard deviation of an array of decimal values.
    /// </summary>
    /// <param name="values">The array of decimal values.</param>
    /// <param name="calculationType">The type of calculation (sample or population).</param>
    /// <returns>The standard deviation of the values.</returns>
    /// <exception cref="EmptyOrNullArrayException">Thrown when the input array is empty.</exception>
    /// <exception cref="InsufficientDataException">Thrown when the input array contains less than two elements for sample calculation.</exception>

    public static decimal StandardDeviation(this decimal[] values, CalculationType calculationType = CalculationType.Sample)
    {
        return (decimal)Math.Sqrt((double)values.Variance(calculationType));
    }
}
