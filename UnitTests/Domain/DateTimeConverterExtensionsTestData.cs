// Copyright (c) 2023-2024 Pierre G. Boutquin. All rights reserved.
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
