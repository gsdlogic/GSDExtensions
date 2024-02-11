// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundexTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="Soundex" /> class.
/// </summary>
public static class SoundexTests
{
    /// <summary>
    /// Ensures that a Soundex encoded value is compared correctly.
    /// </summary>
    [Fact]
    public static void ComparesSoundex()
    {
        Assert.Equal(4, Soundex.Difference("Ashcraft", "Ashcroft"));
        Assert.Equal(4, Soundex.Difference("Smithers", "Smothers"));
        Assert.Equal(4, Soundex.Difference("Clair", "Claire"));
        Assert.Equal(4, Soundex.Difference("Clair", "Clare"));
        Assert.Equal(4, Soundex.Difference("Zach", "Zac"));
        Assert.Equal(3, Soundex.Difference("Jeff", "Geoffe"));
        Assert.Equal(3, Soundex.Difference("Lake", "Bake"));
        Assert.Equal(2, Soundex.Difference("Brad", "Lad"));
        Assert.Equal(2, Soundex.Difference("Zach", "Brad"));
        Assert.Equal(1, Soundex.Difference("Brad", "Zach"));
        Assert.Equal(1, Soundex.Difference("Horrible", "Great"));
    }

    /// <summary>
    /// Ensures that a value is encoded correctly using the Soundex algorithm.
    /// </summary>
    [Fact]
    public static void EncodesSoundex()
    {
        Assert.Equal("0000", Soundex.Encode(null));
        Assert.Equal("A261", Soundex.Encode("Ashcraft"));
        Assert.Equal("A261", Soundex.Encode("Ashcroft"));
        Assert.Equal("C460", Soundex.Encode("Clair"));
        Assert.Equal("C460", Soundex.Encode("Claire"));
        Assert.Equal("C460", Soundex.Encode("Clare"));
        Assert.Equal("D543", Soundex.Encode("Donald"));
        Assert.Equal("C514", Soundex.Encode("Campbel"));
        Assert.Equal("D130", Soundex.Encode("David"));
        Assert.Equal("H555", Soundex.Encode("Honeyman"));
        Assert.Equal("P236", Soundex.Encode("Pfister"));
        Assert.Equal("R163", Soundex.Encode("Robert"));
        Assert.Equal("R150", Soundex.Encode("Rubin"));
        Assert.Equal("R163", Soundex.Encode("Rupert"));
        Assert.Equal("S530", Soundex.Encode("Smith"));
        Assert.Equal("S530", Soundex.Encode("Smythe"));
        Assert.Equal("T522", Soundex.Encode("Tymczak"));
        Assert.Equal("Z200", Soundex.Encode("Zach"));
        Assert.Equal("Z200", Soundex.Encode("Zack"));
    }
}