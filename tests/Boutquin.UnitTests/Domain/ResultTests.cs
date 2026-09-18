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

using Boutquin.Domain.Abstractions;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for <see cref="Result"/>, <see cref="Result{TValue}"/>,
/// and <see cref="ResultExtensions"/>.
/// </summary>
public sealed class ResultTests
{
    private static readonly Error s_testError = new("Test.Error", "A test error occurred");

    // ---- Result (non-generic) ----

    /// <summary>
    /// Tests that Result.Success() creates a successful result with Error.None.
    /// </summary>
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithErrorNone()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }

    /// <summary>
    /// Tests that Result.Failure() creates a failed result with the provided error.
    /// </summary>
    [Fact]
    public void Failure_ShouldCreateFailedResultWithProvidedError()
    {
        // Act
        var result = Result.Failure(s_testError);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_testError);
    }

    /// <summary>
    /// Tests that a successful Result has IsSuccess=true and IsFailure=false.
    /// </summary>
    [Fact]
    public void SuccessfulResult_ShouldHaveIsSuccessTrueAndIsFailureFalse()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    /// <summary>
    /// Tests that a failed Result has IsSuccess=false and IsFailure=true.
    /// </summary>
    [Fact]
    public void FailedResult_ShouldHaveIsSuccessFalseAndIsFailureTrue()
    {
        // Act
        var result = Result.Failure(s_testError);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    /// <summary>
    /// Tests that constructing a <see cref="Result{TValue}"/> with isSuccess=true and a non-None error
    /// throws <see cref="InvalidOperationException"/> — the "a successful result must not carry an error"
    /// invariant. This branch is only reachable through the internal constructor (exposed via
    /// InternalsVisibleTo), since no public factory can produce a success-with-error pair.
    /// </summary>
    [Fact]
    public void InternalConstructor_SuccessWithNonNoneError_ShouldThrowInvalidOperationException()
    {
        // Act
        var act = () => new Result<int>(42, isSuccess: true, error: s_testError);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A successful result should not have an error.");
    }

    /// <summary>
    /// Tests that Result.Failure() with Error.None throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void Failure_WithErrorNone_ShouldThrowInvalidOperationException()
    {
        // Act
        var act = () => Result.Failure(Error.None);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A failed result must have an error.");
    }

    // ---- Result<TValue> ----

    /// <summary>
    /// Tests that Result&lt;T&gt;.Success(value) contains the correct Value.
    /// </summary>
    [Fact]
    public void GenericSuccess_ShouldHaveCorrectValue()
    {
        // Arrange
        const int expected = 42;

        // Act
        var result = Result.Success(expected);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }

    /// <summary>
    /// Tests that accessing Value on a failed Result&lt;T&gt; throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void GenericFailure_AccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var result = Result.Failure<int>(s_testError);

        // Act
        var act = () => result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The value of a failure result cannot be accessed.");
    }

    /// <summary>
    /// Tests that Result.Create with a non-null value returns a success result.
    /// </summary>
    [Fact]
    public void Create_WithNonNullValue_ShouldReturnSuccess()
    {
        // Arrange
        const string value = "hello";

        // Act
        var result = Result.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    /// <summary>
    /// Tests that Result.Create with a null value returns a failure with Error.NullValue.
    /// </summary>
    [Fact]
    public void Create_WithNullValue_ShouldReturnFailureWithNullValueError()
    {
        // Arrange
        string? value = null;

        // Act
        var result = Result.Create(value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    /// <summary>
    /// Tests that implicit conversion from TValue to Result&lt;TValue&gt; works correctly.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
    {
        // Arrange
        const string value = "test";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    /// <summary>
    /// Tests that implicit conversion from a null TValue produces a failure carrying Error.NullValue,
    /// not a success holding null.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromNullValue_ShouldCreateNullValueFailure()
    {
        // Act
        Result<string> result = (string?)null;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    /// <summary>
    /// Tests that implicit conversion from an Error to Result&lt;TValue&gt; produces a failure.
    /// </summary>
    [Fact]
    public void ImplicitConversion_ErrorToGenericResult_ShouldCreateFailure()
    {
        // Act
        Result<int> result = s_testError;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_testError);
    }

    /// <summary>
    /// Tests that implicit conversion from an Error to a non-generic Result produces a failure.
    /// </summary>
    [Fact]
    public void ImplicitConversion_ErrorToResult_ShouldCreateFailure()
    {
        // Act
        Result result = s_testError;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_testError);
    }

    /// <summary>
    /// Tests that Result.Success&lt;TValue&gt; rejects a null reference value with ArgumentNullException.
    /// </summary>
    [Fact]
    public void GenericSuccess_WithNullValue_ThrowsArgumentNullException()
    {
        // Act
        var act = () => Result.Success<string>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("value");
    }

    /// <summary>
    /// Tests the value-type boundary: Result.Success(0) is a success carrying 0 (the null check is a
    /// no-op for non-nullable value types).
    /// </summary>
    [Fact]
    public void GenericSuccess_WithZeroValueType_IsSuccessCarryingZero()
    {
        // Act
        var result = Result.Success(0);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }

    // ---- ResultExtensions.Match ----

    /// <summary>
    /// Tests that Match on a successful Result calls the onSuccess function.
    /// </summary>
    [Fact]
    public void Match_OnSuccessfulResult_ShouldCallOnSuccess()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var message = result.Match(
            onSuccess: () => "success",
            onFailure: error => $"failure: {error.Description}");

        // Assert
        message.Should().Be("success");
    }

    /// <summary>
    /// Tests that Match on a failed Result calls the onFailure function.
    /// </summary>
    [Fact]
    public void Match_OnFailedResult_ShouldCallOnFailure()
    {
        // Arrange
        var result = Result.Failure(s_testError);

        // Act
        var message = result.Match(
            onSuccess: () => "success",
            onFailure: error => $"failure: {error.Description}");

        // Assert
        message.Should().Be($"failure: {s_testError.Description}");
    }

    /// <summary>
    /// Tests that Match on a successful Result&lt;TValue&gt; calls onSuccess with the value.
    /// </summary>
    [Fact]
    public void Match_OnSuccessfulGenericResult_ShouldCallOnSuccessWithValue()
    {
        // Arrange
        var result = Result.Success(42);

        // Act
        var message = result.Match(
            onSuccess: value => $"value: {value}",
            onFailure: error => $"failure: {error.Description}");

        // Assert
        message.Should().Be("value: 42");
    }

    /// <summary>
    /// Tests that Match on a failed Result&lt;TValue&gt; calls onFailure with the error.
    /// </summary>
    [Fact]
    public void Match_OnFailedGenericResult_ShouldCallOnFailureWithError()
    {
        // Arrange
        var result = Result.Failure<int>(s_testError);

        // Act
        var message = result.Match(
            onSuccess: value => $"value: {value}",
            onFailure: error => $"failure: {error.Description}");

        // Assert
        message.Should().Be($"failure: {s_testError.Description}");
    }

    /// <summary>
    /// Tests that constructing a successful result with a non-None error
    /// throws InvalidOperationException via the generic Failure factory with Error.None.
    /// </summary>
    [Fact]
    public void GenericFailure_WithErrorNone_ShouldThrowInvalidOperationException()
    {
        // Act
        var act = () => Result.Failure<string>(Error.None);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A failed result must have an error.");
    }

    /// <summary>
    /// Tests that the non-generic <see cref="Result.Failure(Error)"/> factory rejects a null error
    /// with <see cref="ArgumentNullException"/> rather than storing null and deferring the failure.
    /// </summary>
    [Fact]
    public void Failure_WithNullError_ThrowsArgumentNullException()
    {
        // Act
        var act = () => Result.Failure(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("error");
    }

    /// <summary>
    /// Tests that the generic <see cref="Result.Failure{TValue}(Error)"/> factory rejects a null error.
    /// </summary>
    [Fact]
    public void GenericFailure_WithNullError_ThrowsArgumentNullException()
    {
        // Act
        var act = () => Result.Failure<string>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("error");
    }

}
