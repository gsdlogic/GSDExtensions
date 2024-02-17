// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteSizeTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="ByteSize" /> class.
/// </summary>
public class ByteSizeTests
{
    /// <summary>
    /// Ensures that the constants are accurate.
    /// </summary>
    [Fact]
    public void ConstantsTests()
    {
        Assert.Equal(1.0 / 8.0, ByteSize.Bit);
        Assert.Equal(0.5, ByteSize.Nibble);
        Assert.Equal(1.0, ByteSize.Byte);
        Assert.Equal(1024.0, ByteSize.KiloByte);
        Assert.Equal(1024.0 * 1024.0, ByteSize.MegaByte);
        Assert.Equal(1024.0 * 1024.0 * 1024.0, ByteSize.GigaByte);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0, ByteSize.TeraByte);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, ByteSize.PetaByte);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, ByteSize.ExaByte);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, ByteSize.ZettaByte);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, ByteSize.YottaByte);
    }

    /// <summary>
    /// Ensures that byte sizes are formatted properly.
    /// </summary>
    [Fact]
    public void FormatTests()
    {
        Assert.Equal("0 B", ByteSize.ToString(0));
        Assert.Equal("1 B", ByteSize.ToString(1));
        Assert.Equal("1023 B", ByteSize.ToString((1 * (long)ByteSize.KiloByte) - 1));
        Assert.Equal("1 KB", ByteSize.ToString(1 * (long)ByteSize.KiloByte));
        Assert.Equal("1023 KB", ByteSize.ToString((1 * (long)ByteSize.MegaByte) - 1));
        Assert.Equal("1 MB", ByteSize.ToString(1 * (long)ByteSize.MegaByte));
        Assert.Equal("1023 MB", ByteSize.ToString((1 * (long)ByteSize.GigaByte) - 1));
        Assert.Equal("1 GB", ByteSize.ToString(1 * (long)ByteSize.GigaByte));
        Assert.Equal("1023 GB", ByteSize.ToString((1 * (long)ByteSize.TeraByte) - 1));
        Assert.Equal("1 TB", ByteSize.ToString(1 * (long)ByteSize.TeraByte));
        Assert.Equal("1023 TB", ByteSize.ToString((1 * (long)ByteSize.PetaByte) - 1));
        Assert.Equal("1 PB", ByteSize.ToString(1 * (long)ByteSize.PetaByte));
        Assert.Equal("1023 PB", ByteSize.ToString((1 * (long)ByteSize.ExaByte) - 1));
        Assert.Equal("1 EB", ByteSize.ToString(1 * (long)ByteSize.ExaByte));
    }
}