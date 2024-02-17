// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativeTimeTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="RelativeTime" /> class.
/// </summary>
public class RelativeTimeTests
{
    /// <summary>
    /// Ensures that the constants are accurate.
    /// </summary>
    [Fact]
    public void ConstantsTest()
    {
        Assert.Equal(1, RelativeTime.Second);
        Assert.Equal(1 * 60, RelativeTime.Minute);
        Assert.Equal(1 * 60 * 60, RelativeTime.Hour);
        Assert.Equal(1 * 60 * 60 * 24, RelativeTime.Day);
        Assert.Equal(1 * 60 * 60 * 24 * 7, RelativeTime.Week);
        Assert.Equal(1 * 60 * 60 * 24 * (365.25 / 12), RelativeTime.Month);
        Assert.Equal(1 * 60 * 60 * 24 * 365.25, RelativeTime.Year);
    }

    /// <summary>
    /// Ensures that relative <see cref="DateTimeOffset" /> values are accurate.
    /// </summary>
    [Fact]
    public void DateTimeOffsetTests()
    {
        var past = new DateTimeOffset(2024, 1, 1, 1, 10, 0, TimeSpan.Zero);
        var present = new DateTimeOffset(2024, 1, 1, 1, 30, 0, TimeSpan.Zero);
        var future = new DateTimeOffset(2024, 1, 1, 1, 50, 0, TimeSpan.Zero);

        Assert.Equal("20 minutes ago", RelativeTime.ToString(past, present, "ago", "now", "from now"));
        Assert.Equal("now", RelativeTime.ToString(present, present, "ago", "now", "from now"));
        Assert.Equal("20 minutes from now", RelativeTime.ToString(future, present, "ago", "now", "from now"));

        Assert.Equal("3 months from now", RelativeTime.ToString(DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds((3 * RelativeTime.Month) + (20 * RelativeTime.Minute)))));
        Assert.Equal("3 months ago", RelativeTime.ToString(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds((3 * RelativeTime.Month) + (20 * RelativeTime.Minute)))));
    }

    /// <summary>
    /// Ensures that relative <see cref="DateTime" /> values are accurate.
    /// </summary>
    [Fact]
    public void DateTimeTests()
    {
        var past = new DateTime(2024, 1, 1, 1, 10, 0);
        var present = new DateTime(2024, 1, 1, 1, 30, 0);
        var future = new DateTime(2024, 1, 1, 1, 50, 0);

        Assert.Equal("20 minutes ago", RelativeTime.ToString(past, present, "ago", "now", "from now"));
        Assert.Equal("now", RelativeTime.ToString(present, present, "ago", "now", "from now"));
        Assert.Equal("20 minutes from now", RelativeTime.ToString(future, present, "ago", "now", "from now"));

        Assert.Equal("3 months from now", RelativeTime.ToString(DateTime.UtcNow.Add(TimeSpan.FromSeconds((3 * RelativeTime.Month) + (20 * RelativeTime.Minute)))));
        Assert.Equal("3 months ago", RelativeTime.ToString(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds((3 * RelativeTime.Month) + (20 * RelativeTime.Minute)))));
    }

    /// <summary>
    /// Ensures that relative time is calculated properly.
    /// </summary>
    [Fact]
    public void FormatTests()
    {
        Assert.Equal("0 seconds", RelativeTime.ToString(TimeSpan.FromSeconds(0)));
        Assert.Equal("1 second", RelativeTime.ToString(TimeSpan.FromSeconds(1)));
        Assert.Equal("2 seconds", RelativeTime.ToString(TimeSpan.FromSeconds(2)));
        Assert.Equal("59 seconds", RelativeTime.ToString(TimeSpan.FromSeconds(59)));
        Assert.Equal("1 minute", RelativeTime.ToString(TimeSpan.FromSeconds(60)));
        Assert.Equal("1 minute", RelativeTime.ToString(TimeSpan.FromSeconds(119)));
        Assert.Equal("2 minutes", RelativeTime.ToString(TimeSpan.FromSeconds(120)));
        Assert.Equal("59 minutes", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Hour - 1)));
        Assert.Equal("1 hour", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Hour)));
        Assert.Equal("1 hour", RelativeTime.ToString(TimeSpan.FromSeconds((2 * RelativeTime.Hour) - 1)));
        Assert.Equal("2 hours", RelativeTime.ToString(TimeSpan.FromSeconds(2 * RelativeTime.Hour)));
        Assert.Equal("23 hours", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Day - 1)));
        Assert.Equal("1 day", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Day)));
        Assert.Equal("1 day", RelativeTime.ToString(TimeSpan.FromSeconds((2 * RelativeTime.Day) - 1)));
        Assert.Equal("2 days", RelativeTime.ToString(TimeSpan.FromSeconds(2 * RelativeTime.Day)));
        Assert.Equal("6 days", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Week - 1)));
        Assert.Equal("1 week", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Week)));
        Assert.Equal("1 week", RelativeTime.ToString(TimeSpan.FromSeconds((2 * RelativeTime.Week) - 1)));
        Assert.Equal("2 weeks", RelativeTime.ToString(TimeSpan.FromSeconds(2 * RelativeTime.Week)));
        Assert.Equal("4 weeks", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Month - 1)));
        Assert.Equal("1 month", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Month)));
        Assert.Equal("1 month", RelativeTime.ToString(TimeSpan.FromSeconds((2 * RelativeTime.Month) - 1)));
        Assert.Equal("2 months", RelativeTime.ToString(TimeSpan.FromSeconds(2 * RelativeTime.Month)));
        Assert.Equal("11 months", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Year - 1)));
        Assert.Equal("1 year", RelativeTime.ToString(TimeSpan.FromSeconds(RelativeTime.Year)));
        Assert.Equal("1 year", RelativeTime.ToString(TimeSpan.FromSeconds((2 * RelativeTime.Year) - 1)));
        Assert.Equal("2 years", RelativeTime.ToString(TimeSpan.FromSeconds(2 * RelativeTime.Year)));
    }
}