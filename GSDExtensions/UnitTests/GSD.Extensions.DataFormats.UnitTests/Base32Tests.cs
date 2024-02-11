// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base32Tests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="Base32" /> class.
/// </summary>
public class Base32Tests
{
    /// <summary>
    /// Ensures that a Base32 string can be decoded.
    /// </summary>
    [Fact]
    public void DecodeTest()
    {
        const string Value = "HZ7USI5EOOGS4CA=";
        byte[] bytes = { 0x3E, 0x7F, 0x49, 0x23, 0xA4, 0x73, 0x8D, 0x2E, 0x08 };
        Assert.Equal(Value, Base32.ToBase32String(bytes));
    }

    /// <summary>
    /// Ensures that a Base32 string can be encoded.
    /// </summary>
    [Fact]
    public void EncodeTest()
    {
        const string Value = "HZ7USI5EOOGS4CA=";
        byte[] bytes = { 0x3E, 0x7F, 0x49, 0x23, 0xA4, 0x73, 0x8D, 0x2E, 0x08 };
        Assert.True(bytes.SequenceEqual(Base32.FromBase32String(Value)));
    }
}