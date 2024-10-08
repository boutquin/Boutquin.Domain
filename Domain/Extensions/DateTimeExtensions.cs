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

namespace Boutquin.Domain.Extensions;

using Helpers;

/// <summary>
/// DateTimeExtensions provides extension methods to convert date/time between different time zones.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts the specified date/time from the source time zone to the target time zone.
    /// </summary>
    /// <param name="dateTime">The date/time to convert.</param>
    /// <param name="sourceTimeZoneId">The source time zone ID, e.g. "UTC" or "America/New_York".</param>
    /// <param name="targetTimeZoneId">The target time zone ID, e.g. "UTC" or "America/New_York".</param>
    /// <returns>The date/time in the target time zone.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either sourceTimeZoneId or targetTimeZoneId is null.
    /// </exception>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when either sourceTimeZoneId or targetTimeZoneId is not a valid time zone identifier.
    /// </exception>
    public static DateTime ConvertTimeZone(this DateTime dateTime, string sourceTimeZoneId, string targetTimeZoneId)
    {
        // Validate input parameters
        Guard.AgainstNull(() => sourceTimeZoneId);
        Guard.AgainstNull(() => targetTimeZoneId);

        // Find the source and target time zones
        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
        var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneId);

        // Convert the date/time to UTC
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);

        // Convert the UTC date/time to the target time zone
        var targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, targetTimeZone);

        return targetDateTime;
    }
}
