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
        Action act = () => Guard.AgainstNull(() => nullValue);
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
        Action act = () => Guard.AgainstNull(() => nonNullValue);

        // Assert
        act.Should().NotThrow();
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
        Action act = () => Guard.AgainstNullOrEmpty(() => nullValue);
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
        Action act = () => Guard.AgainstNullOrEmpty(() => emptyValue);

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
        Action act = () => Guard.AgainstNullOrEmpty(() => nonEmptyValue);

        // Assert
        act.Should().NotThrow();
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
        Action act = () => Guard.Against(condition).With<InvalidOperationException>();

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
        Action act = () => Guard.Against(condition).With<InvalidOperationException>();

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
        Action act = () => Guard.Against(condition).With<InvalidOperationException>(exceptionMessage);

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
        Action act = () => Guard.Against(condition).With<InvalidOperationException>(exceptionMessage);

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
        var condition = true;

        // Act
        Action act = () => Guard.Against(condition).With<InvalidOperationException>(invalidMessage);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Parameter 'exceptionMessage' cannot be null, empty or contain only white-space characters. (Parameter 'exceptionMessage')")
            .And.ParamName.Should().Be("exceptionMessage");
    }

    /// <summary>
    /// Tests that the Guard.GuardCondition.With method throws an InvalidOperationException when the specified exception type doesn't have a constructor that accepts a single string parameter.
    /// </summary>
    [Fact]
    public void WithMessage_WhenExceptionTypeHasNoStringConstructor_ThrowsInvalidOperationException()
    {
        // Arrange
        var condition = true;
        var exceptionMessage = "An error occurred.";

        // Act
        Action act = () => Guard.Against(condition).With<ExceptionWithoutStringConstructor>(exceptionMessage);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"The exception type '{typeof(ExceptionWithoutStringConstructor).FullName}' must have a constructor that accepts a single string parameter.");
    }

    /// <summary>
    /// Tests if the With method throws an ArgumentException when the exception message is null or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WithExceptionAndInnerException_ThrowsArgumentException_WhenExceptionMessageIsEmpty(string invalidMessage)
    {
        // Arrange
        var condition = true;

        // Act
        Action act = () => Guard.Against(condition).With<CustomException>(invalidMessage, new Exception());

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Parameter 'exceptionMessage' cannot be null, empty or contain only white-space characters. (Parameter 'exceptionMessage')");
    }

    /// <summary>
    /// Tests if the With method throws an ArgumentNullException when the inner exception is null.
    /// </summary>
    [Fact]
    public void WithExceptionAndInnerException_ThrowsArgumentNullException_WhenInnerExceptionIsNull()
    {
        // Arrange
        var condition = true;

        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Action act = () => Guard.Against(condition).With<CustomException>("Test message", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Parameter 'innerException' cannot be null. (Parameter 'innerException')");
    }

    /// <summary>
    /// Tests that the With method throws the specified exception with the provided message and inner exception if the condition is true.
    /// </summary>
    [Fact]
    public void With_WithInnerExceptionAndTrueCondition_ThrowsExceptionWithMessageAndInnerException()
    {
        // Arrange
        var condition = true;
        var exceptionMessage = "Custom exception message";
        Exception innerException = new ApplicationException("Inner exception");

        // Act
        Action act = () => Guard.Against(condition).With<CustomException>(exceptionMessage, innerException);

        // Assert
        act.Should().Throw<CustomException>()
            .WithMessage(exceptionMessage)
            .WithInnerException<ApplicationException>()
            .And.Message.Should().Be("Inner exception");
    }

    /// <summary>
    /// Tests that the With method does not throw an exception if the condition is false.
    /// </summary>
    [Fact]
    public void With_WithInnerExceptionAndFalseCondition_DoesNotThrowException()
    {
        // Arrange
        var condition = false;
        var exceptionMessage = "Custom exception message";
        Exception innerException = new ApplicationException("Inner exception");

        // Act
        Action act = () => Guard.Against(condition).With<CustomException>(exceptionMessage, innerException);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the With method throws an InvalidOperationException when the specified exception type doesn't have a constructor that accepts a single string parameter and an Exception parameter.
    /// </summary>
    [Fact]
    public void With_InvalidExceptionType_ThrowsInvalidOperationException()
    {
        // Arrange
        var condition = true;
        var exceptionMessage = "Custom exception message";
        Exception innerException = new ApplicationException("Inner exception");

        // Act
        Action act = () => Guard.Against(condition).With<ExceptionWithoutStringConstructor>(exceptionMessage, innerException);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"The exception type '{typeof(ExceptionWithoutStringConstructor).FullName}' must have a constructor that accepts a single string parameter and an Exception parameter.");
    }
}
