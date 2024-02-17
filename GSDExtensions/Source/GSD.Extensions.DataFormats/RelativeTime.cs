// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativeTime.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats;

using System;

/// <summary>
/// Provides methods to calculate relative time.
/// </summary>
public static class RelativeTime
{
    /// <summary>
    /// The number of seconds in a day.
    /// </summary>
    public const double Day = 24.0 * Hour;

    /// <summary>
    /// The number of seconds in an hour.
    /// </summary>
    public const double Hour = 60.0 * Minute;

    /// <summary>
    /// The number of seconds in a minute.
    /// </summary>
    public const double Minute = 60.0 * Second;

    /// <summary>
    /// The number of seconds in a month.
    /// </summary>
    public const double Month = Year / 12.0;

    /// <summary>
    /// The number of seconds in a second.
    /// </summary>
    public const double Second = 1.0;

    /// <summary>
    /// The number of seconds in a week.
    /// </summary>
    public const double Week = 7.0 * Day;

    /// <summary>
    /// The number of seconds in a year.
    /// </summary>
    public const double Year = 365.25 * Day;

    /// <summary>
    /// Gets a string the represents the relative time.
    /// </summary>
    /// <param name="timeSpan">The relative timespan.</param>
    /// <returns>A string the represents the amount of time that has elapsed since the specified type.</returns>
    public static string ToString(TimeSpan timeSpan)
    {
        var delta = Math.Abs(timeSpan.TotalSeconds);

        return delta switch
        {
            0 => "0 seconds",
            1 => "1 second",
            < 1 * Minute => $"{Math.Abs(timeSpan.Seconds)} seconds",
            < 2 * Minute => "1 minute",
            < 1 * Hour => $"{Math.Abs(timeSpan.Minutes)} minutes",
            < 2 * Hour => "1 hour",
            < 1 * Day => $"{Math.Abs(timeSpan.Hours)} hours",
            < 2 * Day => "1 day",
            < 1 * Week => $"{Math.Abs(timeSpan.Days)} days",
            < 2 * Week => "1 week",
            < 1 * Month => $"{Convert.ToInt32(Math.Floor(delta / Week))} weeks",
            < 2 * Month => "1 month",
            < 1 * Year => $"{Convert.ToInt32(Math.Floor(delta / Month))} months",
            < 2 * Year => "1 year",
            _ => $"{Convert.ToInt32(Math.Floor(delta / Year))} years",
        };
    }

    /// <summary>
    /// Gets a string the represents the relative time.
    /// </summary>
    /// <param name="time">The UTC date and time to compare.</param>
    /// <returns>A string the represents the amount of time that has elapsed since the specified time.</returns>
    public static string ToString(DateTime time)
    {
        return ToString(time, DateTime.UtcNow, "ago", "now", "from now");
    }

    /// <summary>
    /// Gets a string the represents the relative time.
    /// </summary>
    /// <param name="time">The UTC date and time to compare.</param>
    /// <param name="relativeTo">The relative date and time to compare.</param>
    /// <param name="past">The string to append if <paramref name="time" /> is less than <paramref name="relativeTo" />.</param>
    /// <param name="present">The string to append if <paramref name="time" /> is equal to <paramref name="relativeTo" />.</param>
    /// <param name="future">The string to append if <paramref name="time" /> is greater than <paramref name="relativeTo" />.</param>
    /// <returns>A string the represents the amount of time that has elapsed since the specified time.</returns>
    public static string ToString(DateTime time, DateTime relativeTo, string past, string present, string future)
    {
        var timeSpan = new TimeSpan(time.Ticks - relativeTo.Ticks);

        return timeSpan.TotalSeconds switch
        {
            0.0 => present,
            < 0 => $"{ToString(timeSpan)} {past}",
            _ => $"{ToString(timeSpan)} {future}",
        };
    }

    /// <summary>
    /// Gets a string the represents the relative time.
    /// </summary>
    /// <param name="time">The UTC date and time to compare.</param>
    /// <returns>A string the represents the amount of time that has elapsed since the specified time.</returns>
    public static string ToString(DateTimeOffset time)
    {
        return ToString(time, DateTimeOffset.UtcNow, "ago", "now", "from now");
    }

    /// <summary>
    /// Gets a string the represents the relative time.
    /// </summary>
    /// <param name="time">The UTC date and time to compare.</param>
    /// <param name="relativeTo">The relative date and time.</param>
    /// <param name="past">The string to append if <paramref name="time" /> is less than <paramref name="relativeTo" />.</param>
    /// <param name="present">The string to append if <paramref name="time" /> is equal to <paramref name="relativeTo" />.</param>
    /// <param name="future">The string to append if <paramref name="time" /> is greater than <paramref name="relativeTo" />.</param>
    /// <returns>A string the represents the amount of time that has elapsed since the specified time.</returns>
    public static string ToString(DateTimeOffset time, DateTimeOffset relativeTo, string past, string present, string future)
    {
        var timeSpan = new TimeSpan(time.Ticks - relativeTo.Ticks);

        return timeSpan.TotalSeconds switch
        {
            0.0 => present,
            < 0 => $"{ToString(timeSpan)} {past}",
            _ => $"{ToString(timeSpan)} {future}",
        };
    }
}