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
    /// <exception cref="ArgumentException">
    /// Thrown when either sourceTimeZoneId or targetTimeZoneId is empty or white-space, or when
    /// <paramref name="dateTime"/> is a wall-clock time that does not exist in the source zone because it
    /// falls in the DST spring-forward gap (e.g. 02:30 on 2023-03-12 in <c>America/New_York</c>, where the
    /// clocks jump from 02:00 to 03:00) — <see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/>
    /// rejects such a non-existent time.
    /// </exception>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when either sourceTimeZoneId or targetTimeZoneId is non-blank but not a valid time zone identifier.
    /// </exception>
    /// <remarks>
    /// The conversion interprets <paramref name="dateTime"/> as a wall-clock time in
    /// <paramref name="sourceTimeZoneId"/> regardless of its <see cref="DateTime.Kind"/>. The
    /// <see cref="DateTimeKind"/> is normalized to <see cref="DateTimeKind.Unspecified"/> before the
    /// UTC conversion so that a <see cref="DateTimeKind.Utc"/> or <see cref="DateTimeKind.Local"/>
    /// value does not trigger the <see cref="ArgumentException"/> that
    /// <see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/> raises on a Kind/zone mismatch.
    /// <para>
    /// A wall-clock time in the DST spring-forward gap has no valid UTC instant in the source zone;
    /// <see cref="TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo)"/> throws an
    /// <see cref="ArgumentException"/> for it. A fall-back ambiguous hour (a wall-clock time that occurs
    /// twice) is resolved to the standard-time (later) offset rather than throwing.
    /// </para>
    /// </remarks>
    public static DateTime ConvertTimeZone(this DateTime dateTime, string sourceTimeZoneId, string targetTimeZoneId)
    {
        // Validate input parameters. AgainstNull keeps the precise ArgumentNullException for a null id;
        // AgainstNullOrWhiteSpace then rejects empty/white-space ids so a blank id surfaces as a clear
        // ArgumentException rather than an opaque TimeZoneNotFoundException about "".
        Guard.AgainstNull(() => sourceTimeZoneId);
        Guard.AgainstNullOrWhiteSpace(() => sourceTimeZoneId);
        Guard.AgainstNull(() => targetTimeZoneId);
        Guard.AgainstNullOrWhiteSpace(() => targetTimeZoneId);

        // Find the source and target time zones
        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
        var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneId);

        // Treat the input as a wall-clock time in the source zone. ConvertTimeToUtc throws an
        // ArgumentException when dateTime.Kind contradicts the source zone (e.g. Kind=Utc with a
        // non-UTC source); stripping the Kind to Unspecified makes the conversion Kind-agnostic.
        var sourceDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

        // Convert the date/time to UTC
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(sourceDateTime, sourceTimeZone);

        // Convert the UTC date/time to the target time zone
        var targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, targetTimeZone);

        return targetDateTime;
    }
}
