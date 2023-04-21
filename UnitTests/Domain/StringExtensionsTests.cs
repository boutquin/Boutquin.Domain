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

using Boutquin.Domain.Extensions;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for the <see cref="StringExtensions"/> class.
/// </summary>
public sealed class StringExtensionsTests
{
    /// <summary>
    /// Tests that the IsNullOrEmpty extension method returns true when the string is null.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_WhenStringIsNull_ReturnsTrue()
    {
        // Arrange
        string nullString = null;

        // Act
        var result = nullString.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the IsNullOrEmpty extension method returns true when the string is empty.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_WhenStringIsEmpty_ReturnsTrue()
    {
        // Arrange
        var emptyString = string.Empty;

        // Act
        var result = emptyString.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the IsNullOrEmpty extension method returns false when the string is not empty.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_WhenStringIsNotEmpty_ReturnsFalse()
    {
        // Arrange
        var nonEmptyString = "Hello, World!";

        // Act
        var result = nonEmptyString.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the IsNullOrWhiteSpace extension method returns true when the string is null.
    /// </summary>
    [Fact]
    public void IsNullOrWhiteSpace_WhenStringIsNull_ReturnsTrue()
    {
        // Arrange
        string nullString = null;

        // Act
        var result = nullString.IsNullOrWhiteSpace();

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the IsNullOrWhiteSpace extension method returns true when the string is empty.
    /// </summary>
    [Fact]
    public void IsNullOrWhiteSpace_WhenStringIsEmpty_ReturnsTrue()
    {
        // Arrange
        var emptyString = string.Empty;

        // Act
        var result = emptyString.IsNullOrWhiteSpace();

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the IsNullOrWhiteSpace extension method returns true when the string contains only whitespace characters.
    /// </summary>
    [Fact]
    public void IsNullOrWhiteSpace_WhenStringIsWhiteSpace_ReturnsTrue()
    {
        // Arrange
        var whiteSpaceString = " \t\n\r";

        // Act
        var result = whiteSpaceString.IsNullOrWhiteSpace();

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the IsNullOrWhiteSpace extension method returns false when the string contains non-whitespace characters.
    /// </summary>
    [Fact]
    public void IsNullOrWhiteSpace_WhenStringContainsNonWhiteSpaceCharacters_ReturnsFalse()
    {
        // Arrange
        var stringWithNonWhiteSpaceCharacters = "Hello, World!";

        // Act
        var result = stringWithNonWhiteSpaceCharacters.IsNullOrWhiteSpace();

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the ToUppercaseFirst extension method throws an ArgumentNullException when the string is null.
    /// </summary>
    [Fact]
    public void ToUppercaseFirst_WhenStringIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string nullString = null;

        // Act
        var act = () => nullString.ToUppercaseFirst();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*value*");
    }

    /// <summary>
    /// Tests that the ToUppercaseFirst extension method returns an empty string when the string is empty.
    /// </summary>
    [Fact]
    public void ToUppercaseFirst_WhenStringIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var emptyString = string.Empty;

        // Act
        var result = emptyString.ToUppercaseFirst();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the ToUppercaseFirst extension method returns a string with the first character in uppercase when the string has only one character.
    /// </summary>
    [Fact]
    public void ToUppercaseFirst_WhenStringHasOneCharacter_ReturnsUppercaseCharacter()
    {
        // Arrange
        var singleCharString = "a";

        // Act
        var result = singleCharString.ToUppercaseFirst();

        // Assert
        result.Should().Be("A");
    }

    /// <summary>
    /// Tests that the ToUppercaseFirst extension method returns a string with the first character in uppercase when the string has multiple characters.
    /// </summary>
    [Fact]
    public void ToUppercaseFirst_WhenStringHasMultipleCharacters_ReturnsUppercaseFirstCharacter()
    {
        // Arrange
        var multiCharString = "hello, world!";

        // Act
        var result = multiCharString.ToUppercaseFirst();

        // Assert
        result.Should().Be("Hello, world!");
    }

    /// <summary>
    /// Tests that the ToUppercaseFirst extension method does not change the string when the first character is already in uppercase.
    /// </summary>
    [Fact]
    public void ToUppercaseFirst_WhenFirstCharacterIsAlreadyUppercase_ReturnsSameString()
    {
        // Arrange
        var stringWithUppercaseFirst = "Hello, world!";

        // Act
        var result = stringWithUppercaseFirst.ToUppercaseFirst();

        // Assert
        result.Should().Be(stringWithUppercaseFirst);
    }

    /// <summary>
    /// Tests that the ToLowerCaseFirst extension method does not throw an exception when the string is empty.
    /// </summary>
    [Fact]
    public void ToLowerCaseFirst_WhenStringIsEmpty_DoesNotThrow()
    {
        // Arrange
        string emptyString = string.Empty;

        // Act
        var act = () => emptyString.ToLowerCaseFirst();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that the ToLowerCaseFirst extension method throws an ArgumentNullException when the string is null.
    /// </summary>
    [Fact]
    public void ToLowerCaseFirst_WhenStringIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string nullString = null;

        // Act
        var act = () => nullString.ToLowerCaseFirst();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*value*");
    }

    /// <summary>
    /// Tests that the ToLowerCaseFirst extension method returns the correct value when the string has only one character.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="expected">The expected output string.</param>
    [Theory]
    [InlineData("A", "a")]
    [InlineData("Z", "z")]
    [InlineData("a", "a")]
    [InlineData("z", "z")]
    public void ToLowerCaseFirst_WhenStringHasOneCharacter_ReturnsCorrectValue(string input, string expected)
    {
        // Act
        var result = input.ToLowerCaseFirst();

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Tests that the ToLowerCaseFirst extension method returns the correct value when the string has multiple characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="expected">The expected output string.</param>
    [Theory]
    [InlineData("Hello", "hello")]
    [InlineData("World", "world")]
    [InlineData("HELLO", "hELLO")]
    [InlineData("WORLD", "wORLD")]
    public void ToLowerCaseFirst_WhenStringHasMultipleCharacters_ReturnsCorrectValue(string input, string expected)
    {
        // Act
        var result = input.ToLowerCaseFirst();

        // Assert
        result.Should().Be(expected);
    }
}
