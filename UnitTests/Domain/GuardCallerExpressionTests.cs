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

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Contains unit tests for the CallerArgumentExpression-based Guard overloads.
/// </summary>
public sealed class GuardCallerExpressionTests
{
    // ─── AgainstNull ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstNull throws ArgumentNullException when the value is null.
    /// </summary>
    [Fact]
    public void AgainstNull_WhenNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? nullValue = null;

        // Act
        var act = () => Guard.AgainstNull(nullValue);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that AgainstNull does not throw when the value is non-null.
    /// </summary>
    [Fact]
    public void AgainstNull_WhenNotNull_DoesNotThrow()
    {
        // Arrange
        var value = "hello";

        // Act
        var act = () => Guard.AgainstNull(value);

        // Assert
        act.Should().NotThrow();
    }

    // ─── AgainstDefault ──────────────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstDefault throws ArgumentException when the value is default.
    /// </summary>
    [Fact]
    public void AgainstDefault_WhenDefault_ThrowsArgumentException()
    {
        // Arrange
        var value = 0;

        // Act
        var act = () => Guard.AgainstDefault(value);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw when the value is non-default.
    /// </summary>
    [Fact]
    public void AgainstDefault_WhenNonDefault_DoesNotThrow()
    {
        // Arrange
        var value = 42;

        // Act
        var act = () => Guard.AgainstDefault(value);

        // Assert
        act.Should().NotThrow();
    }

    // ─── AgainstNullOrEmpty ──────────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstNullOrEmpty throws ArgumentException when the string is null.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmpty_WhenNull_ThrowsArgumentException()
    {
        // Arrange
        string? nullString = null;

        // Act
        var act = () => Guard.AgainstNullOrEmpty(nullString);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests that AgainstNullOrEmpty throws ArgumentException when the string is empty.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmpty_WhenEmpty_ThrowsArgumentException()
    {
        // Arrange
        var emptyString = string.Empty;

        // Act
        var act = () => Guard.AgainstNullOrEmpty(emptyString);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests that AgainstNullOrEmpty does not throw when the string is valid.
    /// </summary>
    [Fact]
    public void AgainstNullOrEmpty_WhenValid_DoesNotThrow()
    {
        // Arrange
        var value = "hello";

        // Act
        var act = () => Guard.AgainstNullOrEmpty(value);

        // Assert
        act.Should().NotThrow();
    }

    // ─── AgainstNullOrWhiteSpace ─────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstNullOrWhiteSpace throws ArgumentException when the string is whitespace.
    /// </summary>
    [Fact]
    public void AgainstNullOrWhiteSpace_WhenWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var whitespace = "   ";

        // Act
        var act = () => Guard.AgainstNullOrWhiteSpace(whitespace);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // ─── AgainstNegative ─────────────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstNegative throws ArgumentOutOfRangeException when the value is negative.
    /// </summary>
    [Fact]
    public void AgainstNegative_WhenNegative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var value = -1;

        // Act
        var act = () => Guard.AgainstNegative(value);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests that AgainstNegative does not throw when the value is zero.
    /// </summary>
    [Fact]
    public void AgainstNegative_WhenZero_DoesNotThrow()
    {
        // Arrange
        var value = 0;

        // Act
        var act = () => Guard.AgainstNegative(value);

        // Assert
        act.Should().NotThrow();
    }

    // ─── AgainstNegativeOrZero ───────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstNegativeOrZero throws ArgumentOutOfRangeException when the value is zero.
    /// </summary>
    [Fact]
    public void AgainstNegativeOrZero_WhenZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var value = 0;

        // Act
        var act = () => Guard.AgainstNegativeOrZero(value);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests that AgainstNegativeOrZero does not throw when the value is positive.
    /// </summary>
    [Fact]
    public void AgainstNegativeOrZero_WhenPositive_DoesNotThrow()
    {
        // Arrange
        var value = 1;

        // Act
        var act = () => Guard.AgainstNegativeOrZero(value);

        // Assert
        act.Should().NotThrow();
    }

    // ─── AgainstOutOfRange ───────────────────────────────────────────────────────

    /// <summary>
    /// Tests that AgainstOutOfRange throws ArgumentOutOfRangeException when the value is outside the range.
    /// </summary>
    [Fact]
    public void AgainstOutOfRange_WhenOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var value = 100;

        // Act
        var act = () => Guard.AgainstOutOfRange(value, 1, 10);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Tests that AgainstOutOfRange does not throw when the value is within the range.
    /// </summary>
    [Fact]
    public void AgainstOutOfRange_WhenInRange_DoesNotThrow()
    {
        // Arrange
        var value = 5;

        // Act
        var act = () => Guard.AgainstOutOfRange(value, 1, 10);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that AgainstOutOfRange throws ArgumentException when min is greater than or equal to max.
    /// </summary>
    [Fact]
    public void AgainstOutOfRange_WhenMinGreaterThanOrEqualToMax_ThrowsArgumentException()
    {
        // Arrange
        var value = 5;

        // Act
        var act = () => Guard.AgainstOutOfRange(value, 10, 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*'max'*must be greater than*'min'*");
    }
}
