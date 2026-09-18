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

            // Test case 4: ordinary EST (standard time), NOT a DST transition. 2023-03-10 precedes that
            // year's spring-forward (2023-03-12), so America/New_York is still at its UTC-5 standard
            // offset; the 5-hour delta below is the everyday EST offset, not a transition artifact.
            [new DateTime(2023, 3, 10, 2, 0, 0), "America/New_York", "UTC", new DateTime(2023, 3, 10, 7, 0, 0)],

            // Test case 5: DST fall-back ambiguous hour. 2023-11-05 01:30 America/New_York occurs twice
            // (once as EDT before the 2 AM fall-back, once as EST after it); ConvertTimeZone resolves the
            // ambiguity to the standard-time (later, UTC-5) offset rather than throwing, so 01:30 -> 06:30 UTC.
            [new DateTime(2023, 11, 5, 1, 30, 0), "America/New_York", "UTC", new DateTime(2023, 11, 5, 6, 30, 0)]
        ];
}
