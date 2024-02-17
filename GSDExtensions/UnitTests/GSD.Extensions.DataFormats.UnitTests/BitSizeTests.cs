// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitSizeTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="BitSize" /> class.
/// </summary>
public class BitSizeTests
{
    /// <summary>
    /// Ensures that the constants are accurate.
    /// </summary>
    [Fact]
    public void ConstantsTests()
    {
        Assert.Equal(1.0, BitSize.Bit);
        Assert.Equal(4.0, BitSize.Nibble);
        Assert.Equal(8.0, BitSize.Byte);
        Assert.Equal(1024.0, BitSize.KiloBit);
        Assert.Equal(1024.0 * 1024.0, BitSize.MegaBit);
        Assert.Equal(1024.0 * 1024.0 * 1024.0, BitSize.GigaBit);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0, BitSize.TeraBit);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, BitSize.PetaBit);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, BitSize.ExaBit);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, BitSize.ZettaBit);
        Assert.Equal(1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0 * 1024.0, BitSize.YottaBit);
    }

    /// <summary>
    /// Ensures that bit sizes are formatted properly.
    /// </summary>
    [Fact]
    public void FormatTests()
    {
        Assert.Equal("0 b", BitSize.ToString(0));
        Assert.Equal("1 b", BitSize.ToString(1));
        Assert.Equal("1023 b", BitSize.ToString((1 * (long)BitSize.KiloBit) - 1));
        Assert.Equal("1 Kb", BitSize.ToString(1 * (long)BitSize.KiloBit));
        Assert.Equal("1023 Kb", BitSize.ToString((1 * (long)BitSize.MegaBit) - 1));
        Assert.Equal("1 Mb", BitSize.ToString(1 * (long)BitSize.MegaBit));
        Assert.Equal("1023 Mb", BitSize.ToString((1 * (long)BitSize.GigaBit) - 1));
        Assert.Equal("1 Gb", BitSize.ToString(1 * (long)BitSize.GigaBit));
        Assert.Equal("1023 Gb", BitSize.ToString((1 * (long)BitSize.TeraBit) - 1));
        Assert.Equal("1 Tb", BitSize.ToString(1 * (long)BitSize.TeraBit));
        Assert.Equal("1023 Tb", BitSize.ToString((1 * (long)BitSize.PetaBit) - 1));
        Assert.Equal("1 Pb", BitSize.ToString(1 * (long)BitSize.PetaBit));
        Assert.Equal("1023 Pb", BitSize.ToString((1 * (long)BitSize.ExaBit) - 1));
        Assert.Equal("1 Eb", BitSize.ToString(1 * (long)BitSize.ExaBit));
    }
}