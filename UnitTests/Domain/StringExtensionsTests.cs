// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for the <see cref="StringExtensions"/> class.
/// </summary>
/// <remarks>
/// This test class provides test cases for various string extension methods such as:
/// - IsNullOrEmpty: Tests for null or empty strings.
/// - IsNullOrWhiteSpace: Tests for null, empty, or whitespace strings.
/// - ToUppercaseFirst: Tests for converting the first character of a string to uppercase.
/// - ToLowerCaseFirst: Tests for converting the first character of a string to lowercase.
/// </remarks>
public sealed class StringExtensionsTests
{
    /// <summary>
    /// Tests that the IsNullOrEmpty extension method returns true when the string is null.
    /// </summary>
    [Fact]
    public void IsNullOrEmpty_WhenStringIsNull_ReturnsTrue()
    {
        // Arrange
#pragma warning disable CS8600
        string nullString = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8604
        var result = nullString.IsNullOrEmpty();
#pragma warning restore CS8604

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
#pragma warning disable CS8600
        string nullString = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8604
        var result = nullString.IsNullOrWhiteSpace();
#pragma warning restore CS8604

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
#pragma warning disable CS8600
        string nullString = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8604
        var act = nullString.ToUppercaseFirst;
#pragma warning restore CS8604

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
        var emptyString = string.Empty;

        // Act
        var act = emptyString.ToLowerCaseFirst;

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
#pragma warning disable CS8600
        string nullString = null;
#pragma warning restore CS8600

        // Act
#pragma warning disable CS8604
        var act = nullString.ToLowerCaseFirst;
#pragma warning restore CS8604

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
