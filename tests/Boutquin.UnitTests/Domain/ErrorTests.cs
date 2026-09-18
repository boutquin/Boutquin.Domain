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
/// Contains unit tests for the <see cref="Error"/> record.
/// </summary>
public sealed class ErrorTests
{
    /// <summary>
    /// Tests that Error.None has an empty Code and an empty Description.
    /// </summary>
    [Fact]
    public void None_ShouldHaveEmptyCodeAndDescription()
    {
        // Arrange & Act
        var error = Error.None;

        // Assert
        error.Code.Should().BeEmpty();
        error.Description.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that Error.NullValue has the expected Code and Description.
    /// </summary>
    [Fact]
    public void NullValue_ShouldHaveExpectedCodeAndDescription()
    {
        // Arrange & Act
        var error = Error.NullValue;

        // Assert
        error.Code.Should().Be("Error.NullValue");
        error.Description.Should().Be("Null value was provided");
    }

    /// <summary>
    /// Tests that two Error instances with the same Code and Description are equal (record equality).
    /// </summary>
    [Fact]
    public void Errors_WithSameCodeAndDescription_ShouldBeEqual()
    {
        // Arrange
        var error1 = new Error("Test.Error", "A test error");
        var error2 = new Error("Test.Error", "A test error");

        // Act & Assert
        error1.Should().Be(error2);
    }

    /// <summary>
    /// Tests that two Error instances with different Code or Name are not equal.
    /// </summary>
    [Fact]
    public void Errors_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var error1 = new Error("Error.First", "First error");
        var error2 = new Error("Error.Second", "Second error");

        // Act & Assert
        error1.Should().NotBe(error2);
    }

    /// <summary>
    /// Tests that Error.None is not equal to Error.NullValue.
    /// </summary>
    [Fact]
    public void None_ShouldNotEqualNullValue()
    {
        // Act & Assert
        Error.None.Should().NotBe(Error.NullValue);
    }

    /// <summary>
    /// Tests that ErrorType participates in record equality: two errors with the same Code and
    /// Description but different ErrorType are not equal.
    /// </summary>
    [Fact]
    public void Errors_SameCodeAndDescription_DifferentErrorType_AreNotEqual()
    {
        // Arrange
        var a = new Error("Same.Code", "Same description", ErrorType.BadRequest);
        var b = new Error("Same.Code", "Same description", ErrorType.NotFound);

        // Act & Assert
        a.Should().NotBe(b);
    }

    /// <summary>
    /// Tests that an error with empty Code/Description but a non-None ErrorType is not equal to
    /// Error.None (which carries ErrorType.None).
    /// </summary>
    [Fact]
    public void EmptyCodeError_WithNonNoneErrorType_IsNotErrorNone()
    {
        // Arrange
        var error = new Error(string.Empty, string.Empty, ErrorType.BadRequest);

        // Act & Assert
        error.Should().NotBe(Error.None);
    }

    /// <summary>
    /// Tests that every Error factory stamps the corresponding ErrorType.
    /// </summary>
    [Theory]
    [MemberData(nameof(FactoryErrorTypeCases))]
    public void Factory_StampsExpectedErrorType(Error error, ErrorType expected)
    {
        error.ErrorType.Should().Be(expected);
        error.Code.Should().Be("c");
        error.Description.Should().Be("d");
    }

    public static TheoryData<Error, ErrorType> FactoryErrorTypeCases() => new()
    {
        { Error.BadRequest("c", "d"), ErrorType.BadRequest },
        { Error.NotFound("c", "d"), ErrorType.NotFound },
        { Error.Conflict("c", "d"), ErrorType.Conflict },
        { Error.Unauthorized("c", "d"), ErrorType.Unauthorized },
        { Error.Forbidden("c", "d"), ErrorType.Forbidden },
        { Error.RequestTimeout("c", "d"), ErrorType.RequestTimeout },
        { Error.InternalServerError("c", "d"), ErrorType.InternalServerError },
        { Error.Cancelled("c", "d"), ErrorType.Cancelled },
    };
}
