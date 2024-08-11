// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
