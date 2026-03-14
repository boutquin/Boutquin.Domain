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

using System.ComponentModel;

/// <summary>
/// Test enum used to verify GetDescription behavior.
/// </summary>
public enum TestColor
{
    [Description("Bright Red")]
    Red,
    Blue,
    [Description("Forest Green")]
    Green
}

/// <summary>
/// Contains unit tests for the <see cref="EnumExtensions"/> class.
/// </summary>
public sealed class EnumExtensionsTests
{
    /// <summary>
    /// Tests that GetDescription returns the DescriptionAttribute value when present.
    /// </summary>
    [Fact]
    public void GetDescription_WithDescriptionAttribute_ReturnsDescription()
    {
        // Arrange
        var value = TestColor.Red;

        // Act
        var result = value.GetDescription();

        // Assert
        result.Should().Be("Bright Red");
    }

    /// <summary>
    /// Tests that GetDescription returns the enum name when no DescriptionAttribute is present.
    /// </summary>
    [Fact]
    public void GetDescription_WithoutDescriptionAttribute_ReturnsEnumName()
    {
        // Arrange
        var value = TestColor.Blue;

        // Act
        var result = value.GetDescription();

        // Assert
        result.Should().Be("Blue");
    }

    /// <summary>
    /// Tests that GetDescription works correctly across multiple enum values.
    /// </summary>
    [Theory]
    [InlineData(TestColor.Red, "Bright Red")]
    [InlineData(TestColor.Blue, "Blue")]
    [InlineData(TestColor.Green, "Forest Green")]
    public void GetDescription_WithMultipleValues_ReturnsExpected(TestColor value, string expected)
    {
        // Act
        var result = value.GetDescription();

        // Assert
        result.Should().Be(expected);
    }
}
