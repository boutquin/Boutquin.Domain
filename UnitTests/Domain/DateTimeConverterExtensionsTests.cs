// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
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
/// Contains unit tests for the DateTimeConverterExtensions class.
/// </summary>
/// <remarks>
/// This test class provides test cases for the ConvertTimeZone method of the DateTimeConverterExtensions class.
/// It includes tests for various scenarios such as:
/// - Valid conversion of date and time from one time zone to another.
/// - Handling of null target time zone identifier.
/// - Handling of invalid source and target time zone identifiers.
/// </remarks>
public sealed class DateTimeConverterExtensionsTests
{
    /// <summary>
    /// Test cases for the ConvertTimeZone method.
    /// </summary>
    /// <param name="sourceDateTime">The source date/time.</param>
    /// <param name="sourceTimeZoneId">The source time zone ID.</param>
    /// <param name="targetTimeZoneId">The target time zone ID.</param>
    /// <param name="expectedDateTime">The expected date/time after conversion.</param>
    [Theory]
    [MemberData(nameof(DateTimeConverterExtensionsTestData.ConvertTimeZoneCases), MemberType = typeof(DateTimeConverterExtensionsTestData))]
    public void ConvertTimeZone_ShouldReturnCorrectResult(DateTime sourceDateTime, string sourceTimeZoneId, string targetTimeZoneId, DateTime expectedDateTime)
    {
        // Act
        var actualDateTime = sourceDateTime.ConvertTimeZone(sourceTimeZoneId, targetTimeZoneId);

        // Assert
        actualDateTime.Should().Be(expectedDateTime);
    }

    /// <summary>
    /// Test case for the ConvertTimeZone method when the targetTimeZoneId is null.
    /// </summary>
    [Fact]
    public void ConvertTimeZone_ShouldThrowArgumentNullException_WhenTargetTimeZoneIdIsNull()
    {
        // Arrange
        var sourceDateTime = new DateTime(2023, 3, 28, 12, 0, 0);
        const string SourceTimeZoneId = "UTC";
        string? targetTimeZoneId = null;

        // Act and Assert
#pragma warning disable CS8604
        Assert.Throws<ArgumentNullException>(() => sourceDateTime.ConvertTimeZone(SourceTimeZoneId, targetTimeZoneId));
#pragma warning restore CS8604
    }

    /// <summary>
    /// Test case for the ConvertTimeZone method when the sourceTimeZoneId is not a valid time zone identifier.
    /// </summary>
    [Fact]
    public void ConvertTimeZone_ShouldThrowTimeZoneNotFoundException_WhenSourceTimeZoneIdIsInvalid()
    {
        // Arrange
        var sourceDateTime = new DateTime(2023, 3, 28, 12, 0, 0);
        var sourceTimeZoneId = "Invalid/TimeZone";
        var targetTimeZoneId = "UTC";

        // Act and Assert
        Assert.Throws<TimeZoneNotFoundException>(() => sourceDateTime.ConvertTimeZone(sourceTimeZoneId, targetTimeZoneId));
    }

    /// <summary>
    /// Test case for the ConvertTimeZone method when the targetTimeZoneId is not a valid time zone identifier.
    /// </summary>
    [Fact]
    public void ConvertTimeZone_ShouldThrowTimeZoneNotFoundException_WhenTargetTimeZoneIdIsInvalid()
    {
        // Arrange
        var sourceDateTime = new DateTime(2023, 3, 28, 12, 0, 0);
        var sourceTimeZoneId = "UTC";
        var targetTimeZoneId = "Invalid/TimeZone";

        // Act and Assert
        Assert.Throws<TimeZoneNotFoundException>(() => sourceDateTime.ConvertTimeZone(sourceTimeZoneId, targetTimeZoneId));
    }
}
