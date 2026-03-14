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
    /// Tests that Error.None has an empty Code and an empty Name.
    /// </summary>
    [Fact]
    public void None_ShouldHaveEmptyCodeAndName()
    {
        // Arrange & Act
        var error = Error.None;

        // Assert
        error.Code.Should().BeEmpty();
        error.Name.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that Error.NullValue has the expected Code and Name.
    /// </summary>
    [Fact]
    public void NullValue_ShouldHaveExpectedCodeAndName()
    {
        // Arrange & Act
        var error = Error.NullValue;

        // Assert
        error.Code.Should().Be("Error.NullValue");
        error.Name.Should().Be("Null value was provided");
    }

    /// <summary>
    /// Tests that two Error instances with the same Code and Name are equal (record equality).
    /// </summary>
    [Fact]
    public void Errors_WithSameCodeAndName_ShouldBeEqual()
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
}
