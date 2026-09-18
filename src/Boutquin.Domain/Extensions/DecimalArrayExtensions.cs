// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License").
//   You may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

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
    /// Represents the valid calculation types for variance and standard deviation.
    /// </summary>
    public enum CalculationType
    {
        /// <summary>
        /// Treats the input as a <i>sample</i> drawn from a larger population. Variance and standard
        /// deviation divide by <c>n - 1</c> (Bessel's correction), giving an unbiased estimate of the
        /// population's dispersion. Requires at least two elements; a single-element array throws
        /// <see cref="InsufficientDataException"/>.
        /// </summary>
        Sample,

        /// <summary>
        /// Treats the input as the <i>entire population</i>. Variance and standard deviation divide by
        /// <c>n</c>, giving the exact dispersion of the supplied values rather than an estimate.
        /// Defined for any non-empty array.
        /// </summary>
        Population
    }

    /// <summary>
    /// Calculates the variance of an array of decimal values.
    /// </summary>
    /// <param name="values">The array of decimal values.</param>
    /// <param name="calculationType">The type of calculation. Must be a defined
    /// <see cref="CalculationType"/> value.</param>
    /// <returns>The variance of the values.</returns>
    /// <exception cref="EmptyOrNullArrayException">Thrown when the input array is empty.</exception>
    /// <exception cref="InsufficientDataException">Thrown when the input array contains less than two elements for sample calculation.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="calculationType"/>
    /// is not a defined <see cref="CalculationType"/> value.</exception>
    /// <exception cref="OverflowException">Thrown when the sum of squared deviations exceeds the range
    /// of <see cref="decimal"/> (only for inputs whose magnitudes approach <see cref="decimal.MaxValue"/>).</exception>
    public static decimal Variance(this decimal[] values, CalculationType calculationType = CalculationType.Sample)
    {
        // Ensure the input array is not null or empty
        Guard.AgainstEmptyOrNullArray(() => values);

        if (!Enum.IsDefined(calculationType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(calculationType),
                calculationType,
                "Calculation type must be a defined value.");
        }

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
    /// <param name="calculationType">The type of calculation. Must be a defined
    /// <see cref="CalculationType"/> value.</param>
    /// <returns>The standard deviation of the values.</returns>
    /// <remarks>
    /// <b>Precision note:</b> the exact <see cref="decimal"/> variance is converted to
    /// <see cref="double"/> for the square root (there is no exact base-10 <c>decimal</c> square root)
    /// and the result is converted back to <see cref="decimal"/>. The returned value therefore carries
    /// only <see cref="double"/> precision (~15–16 significant digits), not the full 28–29 digits of
    /// <see cref="decimal"/>, and very large variances may lose magnitude. Treat the result as accurate
    /// to roughly <c>double</c> precision rather than exact.
    /// </remarks>
    /// <exception cref="EmptyOrNullArrayException">Thrown when the input array is empty.</exception>
    /// <exception cref="InsufficientDataException">Thrown when the input array contains less than two elements for sample calculation.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="calculationType"/>
    /// is not a defined <see cref="CalculationType"/> value.</exception>
    /// <exception cref="OverflowException">Thrown when the underlying <see cref="Variance"/> calculation's
    /// sum of squared deviations exceeds the range of <see cref="decimal"/> (only for inputs whose
    /// magnitudes approach <see cref="decimal.MaxValue"/>).</exception>
    public static decimal StandardDeviation(this decimal[] values, CalculationType calculationType = CalculationType.Sample)
        => (decimal)Math.Sqrt((double)values.Variance(calculationType));
}
