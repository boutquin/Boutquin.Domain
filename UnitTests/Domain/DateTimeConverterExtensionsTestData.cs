// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Test data for the DateTimeConverterExtensionsTests class.
/// </summary>
public static class DateTimeConverterExtensionsTestData
{
    public static IEnumerable<object[]> ConvertTimeZoneCases =>
        [
            // Test case 1: Normal case
            [new DateTime(2023, 3, 28, 12, 0, 0), "UTC", "America/New_York", new DateTime(2023, 3, 28, 8, 0, 0)],

            // Test case 2: Another normal case
            [new DateTime(2023, 3, 28, 12, 0, 0), "UTC", "Asia/Tokyo", new DateTime(2023, 3, 28, 21, 0, 0)],

            // Test case 3: Same source and target time zones
            [new DateTime(2023, 3, 28, 12, 0, 0), "UTC", "UTC", new DateTime(2023, 3, 28, 12, 0, 0)],

            // Test case 4: Daylight saving time transition
            [new DateTime(2023, 3, 10, 2, 0, 0), "America/New_York", "UTC", new DateTime(2023, 3, 10, 7, 0, 0)]
        ];
}
