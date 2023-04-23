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
using Boutquin.Domain.Helpers;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Test class for the Guard class methods.
/// </summary>
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
        int[] nullArray = null;

        // Act
        var act = () => Guard.AgainstNullOrEmptyArray(() => nullArray);

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
    /// Tests that the Guard.AgainstEmptyOrNullDictionary method does not throw an exception when the dictionary is not null or empty.
    /// </summary>
    [Fact]
    public void AgainstEmptyOrNullDictionary_WhenDictionaryIsNotNullOrEmpty_DoesNotThrow()
    {
        // Arrange
        var nonEmptyDictionary = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };

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
        Dictionary<int, string> nullDictionary = null;

        // Act
        var act = () => Guard.AgainstEmptyOrNullDictionary(() => nullDictionary);

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
        var emptyDictionary = new Dictionary<int, string>();

        // Act
        var act = () => Guard.AgainstEmptyOrNullDictionary(() => emptyDictionary);

        // Assert
        act.Should().Throw<EmptyOrNullDictionaryException>().WithMessage($"Parameter '{nameof(emptyDictionary)}' cannot be null or an empty dictionary.");
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

    // Define a sample enum for testing purposes
    private enum SampleEnum
    {
        Value1,
        Value2,
        Value3
    }

    // Define a non-enum type for testing purposes
    private class SampleClass
    {
    }

    /// <summary>
    /// Tests that the Guard.AgainstNonEnumType method does not throw an exception when the type parameter is an enum.
    /// </summary>
    [Fact]
    public void AgainstNonEnumType_WhenTypeIsEnum_DoesNotThrow()
    {
        // Act
        var act = () => Guard.AgainstNonEnumType<SampleEnum>();

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
        var act = () => Guard.AgainstNonEnumType<SampleClass>();

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage($"The type parameter '{nameof(SampleClass)}' must be an enum.");
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method throws the specified exception when the condition is true.
    /// </summary>
    [Fact]
    public void With_WhenConditionIsTrue_ThrowsSpecifiedException()
    {
        // Arrange
        var condition = true;

        // Act
        var act = () => Guard.Against(condition).With<InvalidOperationException>();

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
        var condition = false;

        // Act
        var act = () => Guard.Against(condition).With<InvalidOperationException>();

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
        var condition = true;
        var exceptionMessage = "An error occurred.";

        // Act
        var act = () => Guard.Against(condition).With<InvalidOperationException>(exceptionMessage);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage(exceptionMessage);
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method does not throw an exception when the condition is false.
    /// </summary>
    [Fact]
    public void WithMessage_WhenConditionIsFalse_DoesNotThrow()
    {
        // Arrange
        var condition = false;
        var exceptionMessage = "An error occurred.";

        // Act
        var act = () => Guard.Against(condition).With<InvalidOperationException>(exceptionMessage);

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
}
