// Copyright (c) 2023-2024 Pierre G. Boutquin. All rights reserved.
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
namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for the DecimalArrayExtensions class.
/// </summary>
/// <remarks>
/// This test class provides test cases for the Variance and StandardDeviation methods of the DecimalArrayExtensions class.
/// It includes tests for various scenarios such as:
/// - Correct calculation of variance and standard deviation for a given array of decimal values and calculation type.
/// - Handling of insufficient data for sample calculation when the input array contains only one element.
/// - Handling of null or empty input arrays.
/// </remarks>
public sealed class DecimalArrayExtensionsTests
{
    private const decimal Precision = 1e-12m;

    /// <summary>
    /// Verifies that the <see cref="DecimalArrayExtensions.Variance(decimal[], CalculationType)"/> extension method 
    /// correctly calculates the variance of an array of decimal values given a calculation type (sample or population).
    /// </summary>
    /// <param name="values">The array of decimal values to calculate the standard deviation for.</param>
    /// <param name="calculationType">The calculation type to use (<see cref="CalculationType.Sample"/> or <see cref="CalculationType.Population"/>).</param>
    /// <param name="expected">The expected standard deviation.</param>
    [Theory]
    [MemberData(nameof(DecimalArrayExtensionsTestData.VarianceData), MemberType = typeof(DecimalArrayExtensionsTestData))]
    public void Variance_ShouldCalculateCorrectly(decimal[] values, CalculationType calculationType, decimal expected)
    {
        // Act
        var result = values.Variance(calculationType);

        // Assert
        result.Should().BeApproximately(expected, Precision);
    }

    /// <summary>
    /// Verifies that the <see cref="DecimalArrayExtensions.StandardDeviation(decimal[], CalculationType)"/> extension method 
    /// correctly calculates the standard deviation of an array of decimal values given a calculation type (sample or population).
    /// </summary>
    /// <param name="values">The array of decimal values to calculate the standard deviation for.</param>
    /// <param name="calculationType">The calculation type to use (<see cref="CalculationType.Sample"/> or <see cref="CalculationType.Population"/>).</param>
    /// <param name="expected">The expected standard deviation.</param>
    [Theory]
    [MemberData(nameof(DecimalArrayExtensionsTestData.StandardDeviationData), MemberType = typeof(DecimalArrayExtensionsTestData))]
    public void StandardDeviation_ShouldCalculateCorrectly(decimal[] values, CalculationType calculationType, decimal expected)
    {
        // Act
        var result = values.StandardDeviation(calculationType);

        // Assert
        result.Should().BeApproximately(expected, Precision);
    }

    /// <summary>
    /// Tests the <see cref="InsufficientDataException" /> for all extension methods 
    /// that require <see cref="CalculationType.Sample"/> calculations with an input array containing only one element.
    /// </summary>
    [Fact]
    public void AllMethods_ShouldThrowInsufficientDataForSampleCalculation_WhenArrayHasOneElement()
    {
        var values = new[] { 0.01m };
        var exceptionType = typeof(InsufficientDataException);
        var exceptionMessage = ExceptionMessages.InsufficientDataForSampleCalculation;

        // Act & Assert
        Assert.Throws(exceptionType, () => values.Variance()).Message.Should().Be(exceptionMessage);
        Assert.Throws(exceptionType, () => values.StandardDeviation()).Message.Should().Be(exceptionMessage);
    }

    /// <summary>
    /// Tests the <see cref="EmptyOrNullArrayException" /> for all extension methods with null input arrays.
    /// </summary>
    [Fact]
    public void AllMethods_ShouldThrowEmptyOrNullArrayException_WhenArrayIsNull()
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        // Arrange
        decimal[] values = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        var exceptionType = typeof(EmptyOrNullArrayException);
        const string ExceptionMessage = $"Parameter '{nameof(values)}' cannot be null or an empty array.";

#pragma warning disable CS8604 // Possible null reference argument.
        // Act & Assert
        Assert.Throws(exceptionType, () => values.Variance()).Message.Should().Be(ExceptionMessage);
        Assert.Throws(exceptionType, () => values.StandardDeviation()).Message.Should().Be(ExceptionMessage);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    /// <summary>
    /// Tests the <see cref="EmptyOrNullArrayException" /> for all extension methods with empty input arrays.
    /// </summary>
    [Fact]
    public void AllMethods_ShouldThrowEmptyOrNullArrayException_WhenArrayIsEmpty()
    {
        // Arrange
        var values = Array.Empty<decimal>();
        var exceptionType = typeof(EmptyOrNullArrayException);
        const string ExceptionMessage = $"Parameter '{nameof(values)}' cannot be null or an empty array.";

        // Act & Assert
        Assert.Throws(exceptionType, () => values.Variance()).Message.Should().Be(ExceptionMessage);
        Assert.Throws(exceptionType, () => values.StandardDeviation()).Message.Should().Be(ExceptionMessage);
    }
}
