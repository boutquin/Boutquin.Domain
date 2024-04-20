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

using System.Diagnostics.CodeAnalysis;
using Boutquin.Domain.Exceptions;
using Boutquin.Domain.Helpers;

/// <summary>
/// Contains unit tests for the Guard class.
/// </summary>
/// <remarks>
/// This test class provides test cases for various guard methods such as:
/// - AgainstNull: Tests for null values.
/// - AgainstNullOrEmptyArray: Tests for null or empty arrays.
/// - AgainstEmptyOrNullCollection: Tests for null or empty collections.
/// - AgainstEmptyOrNullDictionary: Tests for null or empty dictionaries.
/// - AgainstNullOrEmpty: Tests for null or empty strings.
/// - AgainstNonEnumType: Tests for non-enum types.
/// - With: Tests for conditions that should throw exceptions.
/// </remarks>
public sealed class GuardTests
{
    /// <summary>
    /// Tests that the Guard.AgainstNull method throws an ArgumentNullException when the value is null.
    /// </summary>
    [Fact]
    public void AgainstNull_WhenValueIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? nullValue = null;

        // Act
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
#pragma warning disable CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
        var act = () => Guard.AgainstNull(() => nullValue);
#pragma warning restore CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage($"Parameter '{nameof(nullValue)}' cannot be null. (Parameter '{nameof(nullValue)}')")
            .And.ParamName.Should().Be(nameof(nullValue));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNull method does not throw an exception when the value is not null.
    /// </summary>
    [Fact]
    public void AgainstNull_WhenValueIsNotNull_DoesNotThrow()
    {
        // Arrange
        var nonNullValue = "Some value";

        // Act
        var act = () => Guard.AgainstNull(() => nonNullValue);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullEnumerable method throws an exception when the enumerable is null.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullEnumerable_WhenEnumerableIsNull_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<int> nullEnumerable = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Guard.AgainstEmptyOrNullEnumerable(() => nullEnumerable));
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullEnumerable method throws an exception when the enumerable is empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullEnumerable_WhenEnumerableIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var emptyEnumerable = new List<int>();

        // Act
        // Expecting ArgumentException instead of InvalidOperationException.
        var exception = Record.Exception(() => Guard.AgainstEmptyOrNullEnumerable(() => emptyEnumerable));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullEnumerable method does not throw an exception when the enumerable is not empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullEnumerable_WhenEnumerableIsNotEmpty_DoesNotThrow()
    {
        // Arrange
        IEnumerable<int> notEmptyEnumerable = new List<int> { 1 };

        // Act
        // Ensure direct member access for compatibility with ExtractParameterInfo
        var exception = Record.Exception(() => Guard.AgainstEmptyOrNullEnumerable(() => notEmptyEnumerable));

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyArray method does not throw an exception when the array is not null or empty.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyArray_WhenArrayIsNotNullOrEmpty_DoesNotThrow()
    {
        // Arrange
        var nonEmptyArray = new[] { 1, 2, 3 };

        // Act
        var act = () => Guard.AgainstNullOrEmptyArray(() => nonEmptyArray);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyArray method throws an ArgumentException when the array is null.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyArray_WhenArrayIsNull_ThrowsArgumentException()
    {
        // Arrange
#pragma warning disable CS8600
        int[] nullArray = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8603
        var act = () => Guard.AgainstNullOrEmptyArray(() => nullArray);
#pragma warning restore CS8603

        // Assert
        act.Should().Throw<EmptyOrNullArrayException>().WithMessage($"Parameter '{nameof(nullArray)}' cannot be null or an empty array.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyArray method throws an ArgumentException when the array is empty.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyArray_WhenArrayIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var emptyArray = Array.Empty<int>();

        // Act
        var act = () => Guard.AgainstNullOrEmptyArray(() => emptyArray);

        // Assert
        act.Should().Throw<EmptyOrNullArrayException>().WithMessage($"Parameter '{nameof(emptyArray)}' cannot be null or an empty array.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullCollection method does not throw an exception when the collection is not null or empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullCollection_WhenCollectionIsNotNullOrEmpty_DoesNotThrow()
    {
        // Arrange
        var nonEmptyList = new List<int> { 1, 2, 3 };

        // Act
        var act = () => Guard.AgainstEmptyOrNullCollection(() => nonEmptyList);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullCollection method throws an EmptyOrNullCollectionException when the collection is null.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullCollection_WhenCollectionIsNull_ThrowsEmptyOrNullCollectionException()
    {
        // Arrange
#pragma warning disable CS8600
        List<int> nullList = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8603
        var act = () => Guard.AgainstEmptyOrNullCollection(() => nullList);
#pragma warning restore CS8603

        // Assert
        act.Should().Throw<EmptyOrNullCollectionException>().WithMessage($"Parameter '{nameof(nullList)}' cannot be null or an empty collection.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullCollection method throws an EmptyOrNullCollectionException when the collection is empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullCollection_WhenCollectionIsEmpty_ThrowsEmptyOrNullCollectionException()
    {
        // Arrange
        var emptyList = new List<int>();

        // Act
        var act = () => Guard.AgainstEmptyOrNullCollection(() => emptyList);

        // Assert
        act.Should().Throw<EmptyOrNullCollectionException>().WithMessage($"Parameter '{nameof(emptyList)}' cannot be null or an empty collection.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullDictionary method does not throw an exception when the dictionary is not null or empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullDictionary_WhenDictionaryIsNotNullOrEmpty_DoesNotThrow()
    {
        // Arrange
        IDictionary<int, string> nonEmptyDictionary = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };

        // Act
        var act = () => Guard.AgainstEmptyOrNullDictionary(() => nonEmptyDictionary);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullDictionary method throws an EmptyOrNullDictionaryException when the dictionary is null.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullDictionary_WhenDictionaryIsNull_ThrowsEmptyOrNullDictionaryException()
    {
        // Arrange
#pragma warning disable CS8600
        IDictionary<int, string> nullDictionary = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8603
        var act = () => Guard.AgainstEmptyOrNullDictionary(() => nullDictionary);
#pragma warning restore CS8603

        // Assert
        act.Should().Throw<EmptyOrNullDictionaryException>().WithMessage($"Parameter '{nameof(nullDictionary)}' cannot be null or an empty dictionary.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullDictionary method throws an EmptyOrNullDictionaryException when the dictionary is empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullDictionary_WhenDictionaryIsEmpty_ThrowsEmptyOrNullDictionaryException()
    {
        // Arrange
        IDictionary<int, string> emptyDictionary = new Dictionary<int, string>();

        // Act
        var act = () => Guard.AgainstEmptyOrNullDictionary(() => emptyDictionary);

        // Assert
        act.Should().Throw<EmptyOrNullDictionaryException>().WithMessage($"Parameter '{nameof(emptyDictionary)}' cannot be null or an empty dictionary.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullReadOnlyDictionary method throws an exception when the dictionary is null.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullReadOnlyDictionary_WhenDictionaryIsNull_ThrowsEmptyOrNullDictionaryException()
    {
        IReadOnlyDictionary<int, string> nullDictionary = null;

        Assert.Throws<EmptyOrNullDictionaryException>(() => Guard.AgainstEmptyOrNullReadOnlyDictionary(() => nullDictionary));
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullReadOnlyDictionary method throws an exception when the dictionary is empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullReadOnlyDictionary_WhenDictionaryIsEmpty_ThrowsEmptyOrNullDictionaryException()
    {
        var emptyDictionary = new Dictionary<int, string>();

        Assert.Throws<EmptyOrNullDictionaryException>(() => Guard.AgainstEmptyOrNullReadOnlyDictionary(() => emptyDictionary));
    }

    /// <summary>
    /// Tests that the Guard.AgainstEmptyOrNullReadOnlyDictionary method does not throw an exception when the dictionary is not empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullReadOnlyDictionary_WhenDictionaryIsNotEmpty_DoesNotThrow()
    {
        var notEmptyDictionary = new Dictionary<int, string> { { 1, "value" } };

        var exception = Record.Exception(() => Guard.AgainstEmptyOrNullReadOnlyDictionary(() => notEmptyDictionary));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNegative method throws an exception when the number is negative.
    /// </summary>
    [Fact]
    public void AgainstNegative_WhenNumberIsNegative_ThrowsException()
    {
        int negativeNumber = -1;

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNegative(() => negativeNumber));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNegative method does not throw an exception when the number is not negative.
    /// </summary>
    [Fact]
    public void AgainstNegative_WhenNumberIsNotNegative_DoesNotThrow()
    {
        int notNegativeNumber = 1;

        var exception = Record.Exception(() => Guard.AgainstNegative(() =>notNegativeNumber));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstOutOfRange method throws an exception when the number is out of range.
    /// </summary>
    [Fact]
    public void AgainstOutOfRange_WhenNumberIsOutOfRange_ThrowsException()
    {
        int outOfRangeNumber = 11;

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstOutOfRange(() => outOfRangeNumber, 1, 10));
    }

    /// <summary>
    /// Tests that the Guard.AgainstOutOfRange method does not throw an exception when the number is within range.
    /// </summary>
    [Fact]
    public void AgainstOutOfRange_WhenNumberIsWithinRange_DoesNotThrow()
    {
        int withinRangeNumber = 5;

        var exception = Record.Exception(() => Guard.AgainstOutOfRange(() => withinRangeNumber, 1, 10));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNegativeOrZero method throws an ArgumentOutOfRangeException when the number is negative or zero.
    /// </summary>
    [Fact]
    public void AgainstNegativeOrZero_WhenNumberIsNegativeOrZero_ThrowsException()
    {
        int negativeNumber = -1;
        int zeroNumber = 0;

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNegativeOrZero(() => negativeNumber));
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNegativeOrZero(() => zeroNumber));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNegativeOrZero method does not throw an exception when the number is positive.
    /// </summary>
    [Fact]
    public void AgainstNegativeOrZero_WhenNumberIsPositive_DoesNotThrow()
    {
        int positiveNumber = 1;

        var exception = Record.Exception(() => Guard.AgainstNegativeOrZero(() => positiveNumber));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmpty method throws an ArgumentException when the value is null.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmpty_WhenValueIsNull_ThrowsArgumentException()
    {
        // Arrange
        string? nullValue = null;

        // Act
#pragma warning disable CS8603 // Possible null reference return.
        var act = () => Guard.AgainstNullOrEmpty(() => nullValue);
#pragma warning restore CS8603 // Possible null reference return.

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Parameter '{nameof(nullValue)}' cannot be null or empty. (Parameter '{nameof(nullValue)}')")
            .And.ParamName.Should().Be(nameof(nullValue));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmpty method throws an ArgumentException when the value is an empty string.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmpty_WhenValueIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var emptyValue = string.Empty;

        // Act
        var act = () => Guard.AgainstNullOrEmpty(() => emptyValue);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Parameter '{nameof(emptyValue)}' cannot be null or empty. (Parameter '{nameof(emptyValue)}')")
            .And.ParamName.Should().Be(nameof(emptyValue));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmpty method does not throw an exception when the value is a non-empty string.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmpty_WhenValueIsNotEmpty_DoesNotThrow()
    {
        // Arrange
        var nonEmptyValue = "Some value";

        // Act
        var act = () => Guard.AgainstNullOrEmpty(() => nonEmptyValue);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrWhiteSpace method throws an exception when the string is null or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AgainstNullOrWhiteSpace_WhenStringIsNullOrWhiteSpace_ThrowsException(string value)
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrWhiteSpace(() => value));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrWhiteSpace method does not throw an exception when the string is not null or whitespace.
    /// </summary>
    [Fact]
    public void AgainstNullOrWhiteSpace_WhenStringIsNotNullOrWhiteSpace_DoesNotThrow()
    {
        string value = "test";

        var exception = Record.Exception(() => Guard.AgainstNullOrWhiteSpace(() => value));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstOverflow method throws an exception when the string length is greater than the max length.
    /// </summary>
    [Fact]
    public void AgainstOverflow_WhenStringLengthIsGreaterThanMaxLength_ThrowsException()
    {
        string value = "this is a very long string";

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstOverflow(() => value, 10));
    }

    /// <summary>
    /// Tests that the Guard.AgainstOverflow method does not throw an exception when the string length is less than or equal to the max length.
    /// </summary>
    [Fact]
    public void AgainstOverflow_WhenStringLengthIsLessThanOrEqualToMaxLength_DoesNotThrow()
    {
        string value = "short";

        var exception = Record.Exception(() => Guard.AgainstOverflow(() => value, 10));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyAndOverflow method throws an exception when the string is null, or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmptyAndOverflow_WhenStringIsNullOrEmpty_ThrowsException(string value)
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmptyAndOverflow(() => value, 10));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyAndOverflow method throws an exception when the string exceeds the max length.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyAndOverflow_WhenOverflow_ThrowsException()
    {
        string value = "this is a very long string";

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNullOrEmptyAndOverflow(() => value, 10));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyAndOverflow method does not throw an exception when the string is not null, not empty and does not exceed the max length.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyAndOverflow_WhenStringIsValid_DoesNotThrow()
    {
        string value = "test";

        var exception = Record.Exception(() => Guard.AgainstNullOrEmptyAndOverflow(() => value, 10));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrWhiteSpaceAndOverflow method throws an exception when the string is null, or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AgainstNullOrWhiteSpaceAndOverflow_WhenStringIsNullOrWhiteSpaceOrOverflow_ThrowsException(string value)
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrWhiteSpaceAndOverflow(() => value, 10));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrWhiteSpaceAndOverflow method throws an exception when the string exceeds the max length.
    /// </summary>
    [Fact]
    public void AgainstNullOrWhiteSpaceAndOverflow_WhenOverflow_ThrowsException()
    {
        string value = "this is a very long string";

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNullOrWhiteSpaceAndOverflow(() => value, 10));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrWhiteSpaceAndOverflow method does not throw an exception when the string is not null, not whitespace and does not exceed the max length.
    /// </summary>
    [Fact]
    public void AgainstNullOrWhiteSpaceAndOverflow_WhenStringIsValid_DoesNotThrow()
    {
        string value = "test";

        var exception = Record.Exception(() => Guard.AgainstNullOrWhiteSpaceAndOverflow(() => value, 10));

        Assert.Null(exception);
    }

    /// <summary>
    /// Defines the SampleEnum enumeration.
    /// </summary>
    /// <remarks>
    /// This enumeration represents different sample values that can be used for testing purposes.
    /// </remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private enum SampleEnum
    {
        Value1,
        Value2,
        Value3
    }

    /// <summary>
    /// Defines a sample class for testing purposes.
    /// </summary>
    /// <remarks>
    /// This class is used in the GuardTests to test the AgainstNonEnumType method.
    /// </remarks>
    // ReSharper disable once ClassNeverInstantiated.Local
    private class SampleClass
    // ReSharper disable once RedundantTypeDeclarationBody
    {
    }

    /// <summary>
    /// Tests that the Guard.AgainstNonEnumType method does not throw an exception when the type parameter is an enum.
    /// </summary>
    [Fact]
    public void AgainstNonEnumType_WhenTypeIsEnum_DoesNotThrow()
    {
        // Act
        var act = Guard.AgainstNonEnumType<SampleEnum>;

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.AgainstNonEnumType method throws an ArgumentException when the type parameter is not an enum.
    /// </summary>
    [Fact]
    public void AgainstNonEnumType_WhenTypeIsNotEnum_ThrowsArgumentException()
    {
        // Act
        var act = Guard.AgainstNonEnumType<SampleClass>;

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage($"The type parameter '{nameof(SampleClass)}' must be an enum.");
    }

    /// <summary>
    /// Tests that the Guard.AgainstUndefinedEnumValue method throws an exception when the enum value is undefined.
    /// </summary>
    [Fact]
    public void AgainstUndefinedEnumValue_WhenEnumValueIsUndefined_ThrowsException()
    {
        DayOfWeek undefinedEnumValue = (DayOfWeek)8;

        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstUndefinedEnumValue(() => undefinedEnumValue));
    }

    /// <summary>
    /// Tests that the Guard.AgainstUndefinedEnumValue method does not throw an exception when the enum value is defined.
    /// </summary>
    [Fact]
    public void AgainstUndefinedEnumValue_WhenEnumValueIsDefined_DoesNotThrow()
    {
        DayOfWeek definedEnumValue = DayOfWeek.Monday;

        var exception = Record.Exception(() => Guard.AgainstUndefinedEnumValue(() => definedEnumValue));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNull method throws an ArgumentNullException when the argument is null.
    /// </summary>
    [Fact]
    public void AgainstNull_WhenArgumentIsNull_ThrowsArgumentNullException()
    {
        object nullObject = null;

        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(() => nullObject));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNull method does not throw an exception when the argument is not null.
    /// </summary>
    [Fact]
    public void AgainstNull_WhenArgumentIsNotNull_DoesNotThrow()
    {
        var notNullObject = new object();

        var exception = Record.Exception(() => Guard.AgainstNull(() => notNullObject));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyArray method throws an EmptyOrNullArrayException when the array is null.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyArray_WhenArrayIsNull_ThrowsEmptyOrNullArrayException()
    {
        int[] nullArray = null;

        Assert.Throws<EmptyOrNullArrayException>(() => Guard.AgainstNullOrEmptyArray(() => nullArray));
    }

    /// <summary>
    /// Tests that the Guard.AgainstNullOrEmptyArray method does not throw an exception when the array is not empty.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmptyArray_WhenArrayIsNotEmpty_DoesNotThrow()
    {
        var notEmptyArray = new int[1];

        var exception = Record.Exception(() => Guard.AgainstNullOrEmptyArray(() => notEmptyArray));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method throws the specified exception when the condition is true.
    /// </summary>
    [Fact]
    public void With_WhenConditionIsTrue_ThrowsSpecifiedException()
    {
        // Arrange
        const bool Condition = true;

        // Act
        var act = () => Guard.Against(Condition).With<InvalidOperationException>();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method does not throw an exception when the condition is false.
    /// </summary>
    [Fact]
    public void With_WhenConditionIsFalse_DoesNotThrow()
    {
        // Arrange
        const bool Condition = false;

        // Act
        var act = () => Guard.Against(Condition).With<InvalidOperationException>();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method throws the specified exception with the provided message when the condition is true.
    /// </summary>
    [Fact]
    public void WithMessage_WhenConditionIsTrue_ThrowsSpecifiedExceptionWithMessage()
    {
        // Arrange
        const bool Condition = true;
        const string ExceptionMessage = "An error occurred.";

        // Act
        var act = () => Guard.Against(Condition).With<InvalidOperationException>(ExceptionMessage);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage(ExceptionMessage);
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method does not throw an exception when the condition is false.
    /// </summary>
    [Fact]
    public void WithMessage_WhenConditionIsFalse_DoesNotThrow()
    {
        // Arrange
        const bool Condition = false;
        const string ExceptionMessage = "An error occurred.";

        // Act
        var act = () => Guard.Against(Condition).With<InvalidOperationException>(ExceptionMessage);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method throws an ArgumentException when the exceptionMessage is null or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WithMessage_WhenExceptionMessageIsNullOrWhiteSpace_ThrowsArgumentException(string invalidMessage)
    {
        // Arrange
        const bool Condition = true;

        // Act
        var act = () => Guard.Against(Condition).With<InvalidOperationException>(invalidMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Parameter 'exceptionMessage' cannot be null, empty or contain only white-space characters. (Parameter 'exceptionMessage')")
            .And.ParamName.Should().Be("exceptionMessage");
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method throws an InvalidOperationException when the specified exception type doesn't have a constructor that accepts a single string parameter.
    /// </summary>
    [Fact]
    public void WithMessage_WhenExceptionTypeHasNoStringConstructor_ThrowsInvalidOperationException()
    {
        // Arrange
        const bool Condition = true;
        const string ExceptionMessage = "An error occurred.";

        // Act
        var act = () => Guard.Against(Condition).With<ExceptionWithoutStringConstructor>(ExceptionMessage);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"The exception type '{typeof(ExceptionWithoutStringConstructor).FullName}' must have a constructor that accepts a single string parameter.");
    }

    /// <summary>
    /// Tests that the GuardCondition.With method throws the specified exception when the condition is true.
    /// </summary>
    [Fact]
    public void GenericWith_WhenConditionIsTrue_ThrowsSpecifiedException()
    {
        var guardCondition = Guard.Against(true);

        Assert.Throws<InvalidOperationException>(() => guardCondition.With<InvalidOperationException>());
    }

    /// <summary>
    /// Tests that the GuardCondition.With method does not throw an exception when the condition is false.
    /// </summary>
    [Fact]
    public void GenericWith_WhenConditionIsFalse_DoesNotThrow()
    {
        var guardCondition = Guard.Against(false);

        var exception = Record.Exception(() => guardCondition.With<InvalidOperationException>());

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the GuardCondition.With method throws the specified exception with the provided arguments when the condition is true.
    /// </summary>
    [Fact]
    public void StringWithArgs_WhenConditionIsTrue_ThrowsSpecifiedExceptionWithArgs()
    {
        var guardCondition = Guard.Against(true);
        const string ExpectedMessage = "Invalid argument";

        var exception = Assert.Throws<ArgumentException>(() => guardCondition.With<ArgumentException>(ExpectedMessage, "arg1"));
        Assert.Equal(ExpectedMessage, exception.Message);
    }

    /// <summary>
    /// Tests that the GuardCondition.With method does not throw an exception when the condition is false.
    /// </summary>
    [Fact]
    public void StringWithArgs_WhenConditionIsFalse_DoesNotThrow()
    {
        var guardCondition = Guard.Against(false);

        var exception = Record.Exception(() => guardCondition.With<ArgumentException>("Invalid argument", "arg1"));

        Assert.Null(exception);
    }

    /// <summary>
    /// Tests that the GuardCondition.With method throws the specified exception with the provided arguments when the condition is true.
    /// </summary>
    [Fact]
    public void WithArgs_WhenConditionIsTrue_ThrowsSpecifiedExceptionWithArgs()
    {
        var guardCondition = Guard.Against(true);

        var exception = Assert.Throws<ArgumentException>(() => guardCondition.With<ArgumentException>(["arg1", "arg2"]));
        Assert.Contains("arg1", exception.Message);
        Assert.Contains("arg2", exception.Message);
    }

    /// <summary>
    /// Tests that the GuardCondition.With method does not throw an exception when the condition is false.
    /// </summary>
    [Fact]
    public void WithArgs_WhenConditionIsFalse_DoesNotThrow()
    {
        var guardCondition = Guard.Against(false);

        var exception = Record.Exception(() => guardCondition.With<ArgumentException>(["arg1", "arg2"]));

        Assert.Null(exception);
    }
}
