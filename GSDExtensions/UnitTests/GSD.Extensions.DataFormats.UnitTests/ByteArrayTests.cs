// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteArrayTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using System.Runtime.InteropServices;
using System.Text;
using Xunit;

/// <summary>
/// Provides a unit test for the <see cref="ByteArray" /> class.
/// </summary>
public static class ByteArrayTests
{
    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.Compare(byte[], byte[])" /> method.
    /// </summary>
    [Fact]
    public static void Compare()
    {
        Assert.Equal(0, ByteArray.Compare(null, null));
        Assert.Equal(0, ByteArray.Compare(Array.Empty<byte>(), Array.Empty<byte>()));
        Assert.Equal(0, ByteArray.Compare(new byte[] { 0x00 }, new byte[] { 0x00 }));
        Assert.Equal(0, ByteArray.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x02, 0x03 }));

        Assert.True(ByteArray.Compare(Array.Empty<byte>(), null) > 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x00 }, Array.Empty<byte>()) > 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x02, 0x00, 0x00 }, new byte[] { 0x01, 0x02, 0x03 }) > 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x03, 0x00 }, new byte[] { 0x01, 0x02, 0x03 }) > 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x02, 0x04 }, new byte[] { 0x01, 0x02, 0x03 }) > 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x02, 0x03, 0x04 }, new byte[] { 0x01, 0x02, 0x03 }) > 0);

        Assert.True(ByteArray.Compare(null, Array.Empty<byte>()) < 0);
        Assert.True(ByteArray.Compare(Array.Empty<byte>(), new byte[] { 0x00 }) < 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x02, 0x00, 0x00 }) < 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x03, 0x00 }) < 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x02, 0x04 }) < 0);
        Assert.True(ByteArray.Compare(new byte[] { 0x01, 0x02, 0x03 }, new byte[] { 0x01, 0x02, 0x03, 0x04 }) < 0);
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="bool" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesBoolean()
    {
        var tester = new EncodeDecodeTester<bool>(
            ByteArray.GetBytes,
            ByteArray.GetBoolean,
            ByteArray.SetBoolean);

        tester.Test(false, new byte[] { 0x00 });
        tester.Test(true, new byte[] { 0x01 });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="byte" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesByte()
    {
        var tester = new EncodeDecodeTester<byte>(
            ByteArray.GetBytes,
            ByteArray.GetByte,
            ByteArray.SetByte);

        tester.Test(1, new byte[] { 0x01 });
        tester.Test(128, new byte[] { 0x80 });
        tester.Test(byte.MinValue, new byte[] { 0x00 });
        tester.Test(byte.MaxValue, new byte[] { 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="char" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesChar()
    {
        var tester = new EncodeDecodeTester<char>(
            ByteArray.GetBytes,
            ByteArray.GetChar,
            ByteArray.SetChar);

        tester.Test('A', new byte[] { 0x00, 0x41 });
        tester.Test('\u0042', new byte[] { 0x00, 0x42 });
        tester.Test('\u0102', new byte[] { 0x01, 0x02 });
        tester.Test('\u0625', new byte[] { 0x06, 0x25 });
        tester.Test(char.MinValue, new byte[] { 0x00, 0x00 });
        tester.Test(char.MaxValue, new byte[] { 0xFF, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="DateTime" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesDateTime()
    {
        var tester = new EncodeDecodeTester<DateTime>(
            ByteArray.GetBytes,
            ByteArray.GetDateTime,
            ByteArray.SetDateTime);

        tester.Test(new DateTime(0x0102030405060708, DateTimeKind.Unspecified), new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
        tester.Test(new DateTime(0x0102030405060708, DateTimeKind.Local), new byte[] { 0x81, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
        tester.Test(new DateTime(0x0102030405060708, DateTimeKind.Utc), new byte[] { 0x41, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
        tester.Test(DateTime.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(DateTime.MaxValue, new byte[] { 0x2B, 0xCA, 0x28, 0x75, 0xF4, 0x37, 0x3F, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="decimal" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesDecimal()
    {
        var tester = new EncodeDecodeTester<decimal>(
            ByteArray.GetBytes,
            ByteArray.GetDecimal,
            ByteArray.SetDecimal);

        tester.Test(decimal.Zero, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(decimal.One, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(decimal.MinusOne, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 });
        tester.Test(decimal.MinValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0x00, 0x00, 0x00 });
        tester.Test(decimal.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(-279750651405311.6220672444168M, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x80, 0x0D, 0x00, 0x00 });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="double" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesDouble()
    {
        var tester = new EncodeDecodeTester<double>(
            ByteArray.GetBytes,
            ByteArray.GetDouble,
            ByteArray.SetDouble);

        tester.Test(0.0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(1.0, new byte[] { 0x3F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(-1.0, new byte[] { 0xBF, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(8.20788039913184E-304, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
        tester.Test(double.MinValue, new byte[] { 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        tester.Test(double.MaxValue, new byte[] { 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        tester.Test(double.NegativeInfinity, new byte[] { 0xFF, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(double.PositiveInfinity, new byte[] { 0x7F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(double.Epsilon, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
        tester.Test(double.NaN, new byte[] { 0xFF, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="Guid" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesGuid()
    {
        var tester = new EncodeDecodeTester<Guid>(
            ByteArray.GetBytes,
            ByteArray.GetGuid,
            ByteArray.SetGuid);

        tester.Test(Guid.Empty, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(Guid.Parse("04030201-0605-0807-090A-0B0C0D0E0F10"), new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="short" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesInt16()
    {
        var tester = new EncodeDecodeTester<short>(
            ByteArray.GetBytes,
            ByteArray.GetInt16,
            ByteArray.SetInt16);

        tester.Test(0, new byte[] { 0x00, 0x00 });
        tester.Test(1, new byte[] { 0x00, 0x01 });
        tester.Test(-1, new byte[] { 0xFF, 0xFF });
        tester.Test(0x0102, new byte[] { 0x01, 0x02 });
        tester.Test(short.MinValue, new byte[] { 0x80, 0x00 });
        tester.Test(short.MaxValue, new byte[] { 0x7F, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="int" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesInt32()
    {
        var tester = new EncodeDecodeTester<int>(
            ByteArray.GetBytes,
            ByteArray.GetInt32,
            ByteArray.SetInt32);

        tester.Test(0, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        tester.Test(1, new byte[] { 0x00, 0x00, 0x00, 0x01 });
        tester.Test(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
        tester.Test(0x01020304, new byte[] { 0x01, 0x02, 0x03, 0x04 });
        tester.Test(int.MinValue, new byte[] { 0x80, 0x00, 0x00, 0x00 });
        tester.Test(int.MaxValue, new byte[] { 0x7F, 0xFF, 0xFF, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="long" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesInt64()
    {
        var tester = new EncodeDecodeTester<long>(
            ByteArray.GetBytes,
            ByteArray.GetInt64,
            ByteArray.SetInt64);

        tester.Test(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(1, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
        tester.Test(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        tester.Test(0x0102030405060708, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
        tester.Test(long.MinValue, new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(long.MaxValue, new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="sbyte" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesSByte()
    {
        var tester = new EncodeDecodeTester<sbyte>(
            ByteArray.GetBytes,
            ByteArray.GetSByte,
            ByteArray.SetSByte);

        tester.Test(0, new byte[] { 0x00 });
        tester.Test(1, new byte[] { 0x01 });
        tester.Test(sbyte.MinValue, new byte[] { 0x80 });
        tester.Test(sbyte.MaxValue, new byte[] { 0x7F });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="float" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesSingle()
    {
        var tester = new EncodeDecodeTester<float>(
            ByteArray.GetBytes,
            ByteArray.GetSingle,
            ByteArray.SetSingle);

        tester.Test(0.0f, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        tester.Test(1.0f, new byte[] { 0x3F, 0x80, 0x00, 0x00 });
        tester.Test(-1.0f, new byte[] { 0xBF, 0x80, 0x00, 0x00 });
        tester.Test(2.38793926E-38f, new byte[] { 0x01, 0x02, 0x03, 0x04 });
        tester.Test(float.MinValue, new byte[] { 0xFF, 0x7F, 0xFF, 0xFF });
        tester.Test(float.MaxValue, new byte[] { 0x7F, 0x7F, 0xFF, 0xFF });
        tester.Test(float.NegativeInfinity, new byte[] { 0xFF, 0x80, 0x00, 0x00 });
        tester.Test(float.PositiveInfinity, new byte[] { 0x7F, 0x80, 0x00, 0x00 });
        tester.Test(float.Epsilon, new byte[] { 0x00, 0x00, 0x00, 0x01 });
        tester.Test(float.NaN, new byte[] { 0xFF, 0xC0, 0x00, 0x00 });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="ushort" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesUInt16()
    {
        var tester = new EncodeDecodeTester<ushort>(
            ByteArray.GetBytes,
            ByteArray.GetUInt16,
            ByteArray.SetUInt16);

        tester.Test(0, new byte[] { 0x00, 0x00 });
        tester.Test(1, new byte[] { 0x00, 0x01 });
        tester.Test(0x0102, new byte[] { 0x01, 0x02 });
        tester.Test(ushort.MinValue, new byte[] { 0x00, 0x00 });
        tester.Test(ushort.MaxValue, new byte[] { 0xFF, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="uint" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesUInt32()
    {
        var tester = new EncodeDecodeTester<uint>(
            ByteArray.GetBytes,
            ByteArray.GetUInt32,
            ByteArray.SetUInt32);

        tester.Test(0, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        tester.Test(1, new byte[] { 0x00, 0x00, 0x00, 0x01 });
        tester.Test(0x01020304, new byte[] { 0x01, 0x02, 0x03, 0x04 });
        tester.Test(uint.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        tester.Test(uint.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for encoding and decoding <see cref="ulong" /> values.
    /// </summary>
    [Fact]
    public static void EncodesAndDecodesUInt64()
    {
        var tester = new EncodeDecodeTester<ulong>(
            ByteArray.GetBytes,
            ByteArray.GetUInt64,
            ByteArray.SetUInt64);

        tester.Test(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(1, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
        tester.Test(0x0102030405060708, new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 });
        tester.Test(ulong.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        tester.Test(ulong.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.Fill(byte[], int, int, byte[])" /> method.
    /// </summary>
    [Fact]
    public static void Fill()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.Fill(null, 0, 1, null));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.Fill(Array.Empty<byte>(), -1, 1, null));
        Assert.Throws<ArgumentException>(() => ByteArray.Fill(new byte[3], 1, 3, null));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.Fill(Array.Empty<byte>(), -1, 1, 0x01));
        Assert.Throws<ArgumentException>(() => ByteArray.Fill(new byte[3], 1, 3, 0x01));

        // ReSharper disable once JoinDeclarationAndInitializer
        byte[] buffer;

        buffer = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        ByteArray.Fill(buffer, 0, 5, null);
        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, buffer);

        buffer = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        ByteArray.Fill(buffer, 0, 5);
        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, buffer);

        buffer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };
        ByteArray.Fill(buffer, 0, 5, 0x01);
        Assert.Equal(new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01 }, buffer);

        buffer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };
        ByteArray.Fill(buffer, 0, 5, 0x01, 0x02);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x01, 0x02, 0x01 }, buffer);

        buffer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };
        ByteArray.Fill(buffer, 0, 5, 0x01, 0x02, 0x03);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x01, 0x02 }, buffer);

        buffer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };
        ByteArray.Fill(buffer, 1, 3, 0x01, 0x02, 0x03);
        Assert.Equal(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x00 }, buffer);

        buffer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };
        ByteArray.Fill(buffer, 3, 2, 0x01, 0x02, 0x03);
        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02 }, buffer);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.Get7BitEncodedInt32(byte[], int, out int)" /> method.
    /// </summary>
    [Fact]
    public static void Get7BitEncodedInt32()
    {
        int length;

        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.Get7BitEncodedInt32(null, 0, out length));
        Assert.Throws<FormatException>(() => ByteArray.Get7BitEncodedInt32(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }, 0, out length));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.Get7BitEncodedInt32(Array.Empty<byte>(), -1, out length));
        Assert.Throws<ArgumentException>(() => ByteArray.Get7BitEncodedInt32(new byte[] { 0x80, 0x80 }, 0, out length));

        var actual = ByteArray.Get7BitEncodedInt32(new byte[] { 0x80, 0x01 }, 0, out length);
        Assert.Equal(2, length);
        Assert.Equal(128, actual);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.Get7BitLengthEncodedString(byte[], int, Encoding, out int)" /> method.
    /// </summary>
    [Fact]
    public static void Get7BitLengthEncodedString()
    {
        int length;

        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.Get7BitLengthEncodedString(null, 0, Encoding.UTF8, out length));
        Assert.Throws<ArgumentNullException>("encoding", () => ByteArray.Get7BitLengthEncodedString(Array.Empty<byte>(), 0, null, out length));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.Get7BitLengthEncodedString(Array.Empty<byte>(), -1, Encoding.UTF8, out length));
        Assert.Throws<ArgumentOutOfRangeException>(() => ByteArray.Get7BitLengthEncodedString(new byte[] { 0x02, 0x41 }, 0, Encoding.ASCII, out length));

        var actual = ByteArray.Get7BitLengthEncodedString(new byte[] { 0x18, 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x77, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x21, 0x00 }, 0, Encoding.Unicode, out length);
        Assert.Equal(25, length);
        Assert.Equal("Hello world!", actual);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetBytes(byte[], int, int)" /> method.
    /// </summary>
    [Fact]
    public static void GetBytes()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.GetBytes(null, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => ByteArray.GetBytes(new byte[3], -1, 3));
        Assert.Throws<ArgumentException>(() => ByteArray.GetBytes(new byte[3], 1, 3));

        Assert.Equal(Array.Empty<byte>(), ByteArray.GetBytes(Array.Empty<byte>(), 0, 0));
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, ByteArray.GetBytes(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 0, 3));
        Assert.Equal(new byte[] { 0x02, 0x03, 0x04 }, ByteArray.GetBytes(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 1, 3));
        Assert.Equal(new byte[] { 0x03, 0x04, 0x05 }, ByteArray.GetBytes(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 2, 3));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetBytes(object)" /> method.
    /// </summary>
    [Fact]
    public static void GetBytesObject()
    {
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, ByteArray.GetBytes(new TestValue(0x04030201, 0x0605)));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetHexDump(byte[], int, int)" /> method.
    /// </summary>
    [Fact]
    public static void GetHexDump()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.GetHexDump(null, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.GetHexDump(new byte[16], -1, 16));
        Assert.Throws<ArgumentException>(() => ByteArray.GetHexDump(new byte[16], 1, 16));

        Assert.Equal($"00000000  41                                                A               {Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 0, 1));
        Assert.Equal($"00000000                       48                                  H        {Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 7, 1));
        Assert.Equal($"00000000                           49                               I       {Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 8, 1));
        Assert.Equal($"00000000                                                50                 P{Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 15, 1));
        Assert.Equal($"00000000  41 42 43 44 45 46 47 48                           ABCDEFGH        {Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 0, 8));
        Assert.Equal($"00000000                           49 4A 4B 4C 4D 4E 4F 50          IJKLMNOP{Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 8, 8));
        Assert.Equal($"00000000  41 42 43 44 45 46 47 48  49 4A 4B 4C 4D 4E 4F 50  ABCDEFGHIJKLMNOP{Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 0, 16));
        Assert.Equal($"00000000                           41 42 43 44 45 46 47 48          ABCDEFGH{Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 8, 8));
        Assert.Equal($"00000000                           41 42 43 44 45 46 47 48          ABCDEFGH{Environment.NewLine}00000010  49 4A 4B 4C 4D 4E 4F 50                           IJKLMNOP        {Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 8, 16));
        Assert.Equal($"00000010  41 42 43 44 45 46 47 48                           ABCDEFGH        {Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 16, 8));
        Assert.Equal($"00000010  41 42 43 44 45 46 47 48  49 4A 4B 4C 4D 4E 4F 50  ABCDEFGHIJKLMNOP{Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 16, 16));
        Assert.Equal($"00000000  00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F  ................{Environment.NewLine}00000010  10 11 12 13 14 15 16 17  18 19 1A 1B 1C 1D 1E 1F  ................{Environment.NewLine}", ByteArray.GetHexDump(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F }, 0, 32));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetHexString(byte[], int, int)" /> method.
    /// </summary>
    [Fact]
    public static void GetHexString()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.GetHexString(null, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.GetHexString(new byte[16], -1, 16));
        Assert.Throws<ArgumentException>(() => ByteArray.GetHexString(new byte[16], 1, 16));

        Assert.Equal("41", ByteArray.GetHexString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 0, 1));
        Assert.Equal("50", ByteArray.GetHexString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 15, 1));
        Assert.Equal("4142434445464748", ByteArray.GetHexString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 0, 8));
        Assert.Equal("494A4B4C4D4E4F50", ByteArray.GetHexString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 8, 8));
        Assert.Equal("4142434445464748494A4B4C4D4E4F50", ByteArray.GetHexString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50 }, 0, 16));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetNullTerminatedString(byte[], int, Encoding, out int)" /> method.
    /// </summary>
    [Fact]
    public static void GetNullTerminatedString()
    {
        int length;

        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.GetNullTerminatedString(null, 0, Encoding.ASCII, out length));
        Assert.Throws<ArgumentNullException>("encoding", () => ByteArray.GetNullTerminatedString(Array.Empty<byte>(), 0, null, out length));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.GetNullTerminatedString(new byte[16], -1, Encoding.ASCII, out length));

        Assert.Equal("ABCD", ByteArray.GetNullTerminatedString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x00 }, 0, Encoding.ASCII, out length));
        Assert.Equal(5, length);

        Assert.Equal("ABCD", ByteArray.GetNullTerminatedString(new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00 }, 0, Encoding.Unicode, out length));
        Assert.Equal(10, length);

        Assert.Equal("ABCD", ByteArray.GetNullTerminatedString(new byte[] { 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00 }, 0, Encoding.BigEndianUnicode, out length));
        Assert.Equal(10, length);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetObject(byte[], int, Type)" /> method.
    /// </summary>
    [Fact]
    public static void GetObject()
    {
        // ReSharper disable once JoinDeclarationAndInitializer
        TestValue value;

        value = (TestValue)ByteArray.GetObject(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xCC, 0xCC, 0xCC, 0xCC }, 0, typeof(TestValue));
        Assert.Equal(0x04030201, value.IntValue);
        Assert.Equal(0x0605, value.ShortValue);

        value = (TestValue)ByteArray.GetObject(new byte[] { 0xCC, 0xCC, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xCC, 0xCC }, 2, typeof(TestValue));
        Assert.Equal(0x04030201, value.IntValue);
        Assert.Equal(0x0605, value.ShortValue);

        value = (TestValue)ByteArray.GetObject(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 4, typeof(TestValue));
        Assert.Equal(0x04030201, value.IntValue);
        Assert.Equal(0x0605, value.ShortValue);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetObject{T}(byte[], int)" /> method.
    /// </summary>
    [Fact]
    public static void GetObjectT()
    {
        var value = ByteArray.GetObject<TestValue>(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 0);
        Assert.Equal(0x04030201, value.IntValue);
        Assert.Equal(0x0605, value.ShortValue);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.GetString(byte[], int, int, Encoding)" /> method.
    /// </summary>
    [Fact]
    public static void GetString()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.GetString(null, 0, 0, Encoding.ASCII));
        Assert.Throws<ArgumentNullException>("encoding", () => ByteArray.GetString(Array.Empty<byte>(), 0, 1, null));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.GetString(new byte[16], -1, 16, Encoding.ASCII));
        Assert.Throws<ArgumentOutOfRangeException>(() => ByteArray.GetString(new byte[16], 1, 16, Encoding.ASCII));

        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x41, 0x42, 0x43, 0x44 }, 0, 4, Encoding.ASCII));
        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00 }, 0, 8, Encoding.Unicode));
        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44 }, 0, 8, Encoding.BigEndianUnicode));

        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x00 }, 0, 5, Encoding.ASCII));
        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00 }, 0, 10, Encoding.Unicode));
        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00 }, 0, 10, Encoding.BigEndianUnicode));

        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0x41, 0x42, 0x43, 0x44, 0xFF, 0xFF }, 0, 4, Encoding.ASCII));
        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0xFF, 0x41, 0x42, 0x43, 0x44, 0xFF }, 1, 4, Encoding.ASCII));
        Assert.Equal("ABCD", ByteArray.GetString(new byte[] { 0xFF, 0xFF, 0x41, 0x42, 0x43, 0x44 }, 2, 4, Encoding.ASCII));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.IndexOf(byte[], int, int, byte[])" /> method.
    /// </summary>
    [Fact]
    public static void IndexOf()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.IndexOf(null, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.IndexOf(new byte[16], -1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ByteArray.IndexOf(new byte[16], 0, 0));

        Assert.Equal(-1, ByteArray.IndexOf(new byte[] { 0x0A, 0x0D, 0x42, 0x43, 0x44 }, 0, 1, 0x0D, 0x0A));
        Assert.Equal(0, ByteArray.IndexOf(new byte[] { 0x0D, 0x0A, 0x41, 0x42, 0x43, 0x44 }, 0, 1, 0x0D, 0x0A));
        Assert.Equal(6, ByteArray.IndexOf(new byte[] { 0x0D, 0x0A, 0x41, 0x42, 0x43, 0x44, 0x0D, 0x0A }, 1, 1, 0x0D, 0x0A));
        Assert.Equal(4, ByteArray.IndexOf(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x0D, 0x0A }, 0, 1, 0x0D, 0x0A));
        Assert.Equal(3, ByteArray.IndexOf(new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00 }, 0, 1, new byte[2]));
        Assert.Equal(4, ByteArray.IndexOf(new byte[] { 0x00, 0x01, 0x00, 0x02, 0x00, 0x00 }, 0, 2, new byte[2]));
        Assert.Equal(4, ByteArray.IndexOf(new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00 }, 0, 2, new byte[2]));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.IndexOfAny(byte[], int, byte[])" /> method.
    /// </summary>
    [Fact]
    public static void IndexOfAny()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.IndexOfAny(null, 0));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.IndexOfAny(new byte[16], -1));

        Assert.Equal(-1, ByteArray.IndexOfAny(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 }, 0));
        Assert.Equal(0, ByteArray.IndexOfAny(new byte[] { 0x00, 0x41, 0x42, 0x43, 0x44 }, 0));
        Assert.Equal(4, ByteArray.IndexOfAny(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x00 }, 0));

        Assert.Equal(-1, ByteArray.IndexOfAny(new byte[] { 0x00, 0x41, 0x42, 0x43, 0x44, 0x45 }, 1));
        Assert.Equal(1, ByteArray.IndexOfAny(new byte[] { 0x00, 0x00, 0x41, 0x42, 0x43, 0x44 }, 1));
        Assert.Equal(5, ByteArray.IndexOfAny(new byte[] { 0x00, 0x41, 0x42, 0x43, 0x44, 0x00 }, 1));

        Assert.Equal(-1, ByteArray.IndexOfAny(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 }, 0, 0x47, 0x46));
        Assert.Equal(0, ByteArray.IndexOfAny(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 }, 0, 0x47, 0x41));
        Assert.Equal(4, ByteArray.IndexOfAny(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 }, 0, 0x47, 0x45));
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.Set7BitEncodedInt32(byte[], int, int)" /> method.
    /// </summary>
    [Fact]
    public static void Set7BitEncodedInt32()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.Set7BitEncodedInt32(null, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.Set7BitEncodedInt32(Array.Empty<byte>(), -1, 0));
        Assert.Throws<ArgumentException>(() => ByteArray.Set7BitEncodedInt32(new byte[1], 0, 128));
        Assert.Throws<ArgumentException>(() => ByteArray.Set7BitEncodedInt32(new byte[2], 1, 128));

        var buffer = new byte[2];
        Assert.Equal(2, ByteArray.Set7BitEncodedInt32(buffer, 0, 128));
        Assert.Equal(new byte[] { 0x80, 0x01 }, buffer);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.Set7BitLengthEncodedString(byte[], int, string, Encoding)" /> method.
    /// </summary>
    [Fact]
    public static void Set7BitLengthEncodedString()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.Set7BitLengthEncodedString(null, 0, "Hello world!", Encoding.UTF8));
        Assert.Throws<ArgumentNullException>("encoding", () => ByteArray.Set7BitLengthEncodedString(Array.Empty<byte>(), 0, "Hello world!", null));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.Set7BitLengthEncodedString(Array.Empty<byte>(), -1, "Hello world!", Encoding.UTF8));
        Assert.Throws<ArgumentException>(() => ByteArray.Set7BitLengthEncodedString(new byte[2], 0, "AB", Encoding.ASCII));

        var buffer = new byte[25];
        Assert.Equal(25, ByteArray.Set7BitLengthEncodedString(buffer, 0, "Hello world!", Encoding.Unicode));
        Assert.Equal(new byte[] { 0x18, 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x77, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x21, 0x00 }, buffer);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.SetNullTerminatedString(byte[], int, string, Encoding)" /> method.
    /// </summary>
    [Fact]
    public static void SetNullTerminatedString()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.SetNullTerminatedString(null, 0, string.Empty, Encoding.ASCII));
        Assert.Throws<ArgumentNullException>("value", () => ByteArray.SetNullTerminatedString(new byte[1], 0, null, Encoding.ASCII));
        Assert.Throws<ArgumentNullException>("encoding", () => ByteArray.SetNullTerminatedString(new byte[1], 0, string.Empty, null));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.SetNullTerminatedString(new byte[1], -1, string.Empty, Encoding.ASCII));
        Assert.Throws<ArgumentException>(() => ByteArray.SetNullTerminatedString(new byte[1], 1, string.Empty, Encoding.ASCII));

        // ReSharper disable once RedundantAssignment
        byte[] buffer = null;

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(5, ByteArray.SetNullTerminatedString(buffer, 0, "ABCD", Encoding.ASCII));
        Assert.Equal(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x00, 0xCC, 0xCC, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(5, ByteArray.SetNullTerminatedString(buffer, 2, "ABCD", Encoding.ASCII));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x41, 0x42, 0x43, 0x44, 0x00, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(5, ByteArray.SetNullTerminatedString(buffer, 4, "ABCD", Encoding.ASCII));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x41, 0x42, 0x43, 0x44, 0x00 }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(10, ByteArray.SetNullTerminatedString(buffer, 0, "ABCD", Encoding.Unicode));
        Assert.Equal(new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0xCC, 0xCC, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(10, ByteArray.SetNullTerminatedString(buffer, 2, "ABCD", Encoding.Unicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(10, ByteArray.SetNullTerminatedString(buffer, 4, "ABCD", Encoding.Unicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0x00 }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(10, ByteArray.SetNullTerminatedString(buffer, 0, "ABCD", Encoding.BigEndianUnicode));
        Assert.Equal(new byte[] { 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0xCC, 0xCC, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(10, ByteArray.SetNullTerminatedString(buffer, 2, "ABCD", Encoding.BigEndianUnicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(10, ByteArray.SetNullTerminatedString(buffer, 4, "ABCD", Encoding.BigEndianUnicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0x00 }, buffer);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.SetObject(byte[], int, object)" /> method.
    /// </summary>
    [Fact]
    public static void SetObject()
    {
        var buffer1 = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(6, ByteArray.SetObject(buffer1, 0, new TestValue(0x04030201, 0x0605)));
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xCC, 0xCC, 0xCC, 0xCC }, buffer1);

        var buffer2 = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(6, ByteArray.SetObject(buffer2, 2, new TestValue(0x04030201, 0x0605)));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xCC, 0xCC }, buffer2);

        var buffer3 = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(6, ByteArray.SetObject(buffer3, 4, new TestValue(0x04030201, 0x0605)));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, buffer3);
    }

    /// <summary>
    /// Provides unit tests for the <see cref="ByteArray.SetString(byte[], int, string, Encoding)" /> method.
    /// </summary>
    [Fact]
    public static void SetString()
    {
        Assert.Throws<ArgumentNullException>("buffer", () => ByteArray.SetString(null, 0, string.Empty, Encoding.ASCII));
        Assert.Throws<ArgumentNullException>("value", () => ByteArray.SetString(new byte[1], 0, null, Encoding.ASCII));
        Assert.Throws<ArgumentNullException>("encoding", () => ByteArray.SetString(new byte[1], 0, string.Empty, null));
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => ByteArray.SetString(Array.Empty<byte>(), -1, string.Empty, Encoding.ASCII));
        Assert.Throws<ArgumentOutOfRangeException>(() => ByteArray.SetString(Array.Empty<byte>(), 1, string.Empty, Encoding.ASCII));

        // ReSharper disable once RedundantAssignment
        byte[] buffer = null;

        buffer = Array.Empty<byte>();
        Assert.Equal(0, ByteArray.SetString(buffer, 0, string.Empty, Encoding.ASCII));
        Assert.Equal(0, ByteArray.SetString(buffer, 0, string.Empty, Encoding.Unicode));
        Assert.Equal(0, ByteArray.SetString(buffer, 0, string.Empty, Encoding.BigEndianUnicode));
        Assert.Equal(0, ByteArray.SetString(buffer, 0, string.Empty, Encoding.UTF8));

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(4, ByteArray.SetString(buffer, 0, "ABCD", Encoding.ASCII));
        Assert.Equal(new byte[] { 0x41, 0x42, 0x43, 0x44, 0xCC, 0xCC, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(4, ByteArray.SetString(buffer, 2, "ABCD", Encoding.ASCII));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x41, 0x42, 0x43, 0x44, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(4, ByteArray.SetString(buffer, 4, "ABCD", Encoding.ASCII));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x41, 0x42, 0x43, 0x44 }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(8, ByteArray.SetString(buffer, 0, "ABCD", Encoding.Unicode));
        Assert.Equal(new byte[] { 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0xCC, 0xCC, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(8, ByteArray.SetString(buffer, 2, "ABCD", Encoding.Unicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(8, ByteArray.SetString(buffer, 4, "ABCD", Encoding.Unicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0x00 }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(8, ByteArray.SetString(buffer, 0, "ABCD", Encoding.BigEndianUnicode));
        Assert.Equal(new byte[] { 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0xCC, 0xCC, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(8, ByteArray.SetString(buffer, 2, "ABCD", Encoding.BigEndianUnicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44, 0xCC, 0xCC }, buffer);

        buffer = new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC };
        Assert.Equal(8, ByteArray.SetString(buffer, 4, "ABCD", Encoding.BigEndianUnicode));
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC, 0x00, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x44 }, buffer);
    }

    /// <summary>
    /// Represents an value for serialization.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 6)]
    private readonly struct TestValue
    {
        /// <summary>
        /// A 32-bit signed integer.
        /// </summary>
        public readonly int IntValue;

        /// <summary>
        /// A 16-bit signed integer.
        /// </summary>
        public readonly short ShortValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestValue" /> struct.
        /// </summary>
        /// <param name="intValue">A 32-bit signed integer.</param>
        /// <param name="shortValue">A 16-bit signed integer.</param>
        public TestValue(int intValue, short shortValue)
        {
            this.IntValue = intValue;
            this.ShortValue = shortValue;
        }
    }

    /// <summary>
    /// Performs tests for encoding an decoding values.
    /// </summary>
    /// <typeparam name="T">The type of value to encode and decode.</typeparam>
    private sealed class EncodeDecodeTester<T>
    {
        /// <summary>
        /// Sets the function to get a byte array from a value.
        /// </summary>
        private readonly Func<T, bool, byte[]> getBigEndianTestBytes;

        /// <summary>
        /// Sets the function to get a value from a byte array.
        /// </summary>
        private readonly Func<byte[], int, bool, T> getBigEndianTestValue;

        /// <summary>
        /// Sets the function to get a byte array from a value.
        /// </summary>
        private readonly Func<T, byte[]> getTestBytes;

        /// <summary>
        /// Sets the function to get a value from a byte array.
        /// </summary>
        private readonly Func<byte[], int, T> getTestValue;

        /// <summary>
        /// Sets the method to store a value in a byte array.
        /// </summary>
        private readonly Action<byte[], int, T, bool> setBigEndianTestValue;

        /// <summary>
        /// Sets the method to store a value in a byte array.
        /// </summary>
        private readonly Action<byte[], int, T> setTestValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodeDecodeTester{T}" /> class.
        /// </summary>
        /// <param name="getTestBytes">The function to get a byte array from a value.</param>
        /// <param name="getTestValue">The function to get a value from a byte array.</param>
        /// <param name="setTestValue">The method to store a value in a byte array.</param>
        public EncodeDecodeTester(
            Func<T, byte[]> getTestBytes,
            Func<byte[], int, T> getTestValue,
            Action<byte[], int, T> setTestValue)
        {
            this.getTestBytes = getTestBytes;
            this.getTestValue = getTestValue;
            this.setTestValue = setTestValue;

            this.TestExceptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodeDecodeTester{T}" /> class.
        /// </summary>
        /// <param name="getBigEndianTestBytes">The function to get a byte array from a value.</param>
        /// <param name="getBigEndianTestValue">The function to get a value from a byte array.</param>
        /// <param name="setBigEndianTestValue">The method to store a value in a byte array.</param>
        public EncodeDecodeTester(
            Func<T, bool, byte[]> getBigEndianTestBytes,
            Func<byte[], int, bool, T> getBigEndianTestValue,
            Action<byte[], int, T, bool> setBigEndianTestValue)
        {
            this.getBigEndianTestBytes = getBigEndianTestBytes;
            this.getBigEndianTestValue = getBigEndianTestValue;
            this.setBigEndianTestValue = setBigEndianTestValue;

            this.TestExceptions();
        }

        /// <summary>
        /// Performs encoding an decoding for a specific value and byte array.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="bytes">The big endian byte array representing the value to test.</param>
        public void Test(T value, byte[] bytes)
        {
            // ReSharper disable once RedundantAssignment
            byte[] buffer = null;

            // ReSharper disable once RedundantAssignment
            byte[] result = null;

            if (this.getTestBytes != null)
            {
                Assert.Equal(bytes, this.getTestBytes(value));
            }

            if (this.getBigEndianTestBytes != null)
            {
                Assert.Equal(bytes, this.getBigEndianTestBytes(value, true));
            }

            buffer = GetPaddedBuffer(bytes, 0, 2);

            if (this.getTestValue != null)
            {
                Assert.Equal(value, this.getTestValue(buffer, 0));
            }

            if (this.getBigEndianTestValue != null)
            {
                Assert.Equal(value, this.getBigEndianTestValue(buffer, 0, true));
            }

            if (this.setTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setTestValue(result, 0, value);
                Assert.Equal(buffer, result);
            }

            if (this.setBigEndianTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setBigEndianTestValue(result, 0, value, true);
                Assert.Equal(buffer, result);
            }

            buffer = GetPaddedBuffer(bytes, 1, 2);

            if (this.getTestValue != null)
            {
                Assert.Equal(value, this.getTestValue(buffer, 1));
            }

            if (this.getBigEndianTestValue != null)
            {
                Assert.Equal(value, this.getBigEndianTestValue(buffer, 1, true));
            }

            if (this.setTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setTestValue(result, 1, value);
                Assert.Equal(buffer, result);
            }

            if (this.setBigEndianTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setBigEndianTestValue(result, 1, value, true);
                Assert.Equal(buffer, result);
            }

            buffer = GetPaddedBuffer(bytes, 2, 2);

            if (this.getTestValue != null)
            {
                Assert.Equal(value, this.getTestValue(buffer, 2));
            }

            if (this.getBigEndianTestValue != null)
            {
                Assert.Equal(value, this.getBigEndianTestValue(buffer, 2, true));
            }

            if (this.setTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setTestValue(result, 2, value);
                Assert.Equal(buffer, result);
            }

            if (this.setBigEndianTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setBigEndianTestValue(result, 2, value, true);
                Assert.Equal(buffer, result);
            }

            Array.Reverse(bytes);

            if (this.getBigEndianTestBytes != null)
            {
                Assert.Equal(bytes, this.getBigEndianTestBytes(value, false));
            }

            buffer = GetPaddedBuffer(bytes, 0, 2);

            if (this.getBigEndianTestValue != null)
            {
                Assert.Equal(value, this.getBigEndianTestValue(buffer, 0, false));
            }

            if (this.setBigEndianTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setBigEndianTestValue(result, 0, value, false);
                Assert.Equal(buffer, result);
            }

            buffer = GetPaddedBuffer(bytes, 1, 2);

            if (this.getBigEndianTestValue != null)
            {
                Assert.Equal(value, this.getBigEndianTestValue(buffer, 1, false));
            }

            if (this.setBigEndianTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setBigEndianTestValue(result, 1, value, false);
                Assert.Equal(buffer, result);
            }

            buffer = GetPaddedBuffer(bytes, 2, 2);

            if (this.getBigEndianTestValue != null)
            {
                Assert.Equal(value, this.getBigEndianTestValue(buffer, 2, false));
            }

            if (this.setBigEndianTestValue != null)
            {
                result = GetFilledBuffer(buffer.Length);
                this.setBigEndianTestValue(result, 2, value, false);
                Assert.Equal(buffer, result);
            }
        }

        /// <summary>
        /// Gets a buffer filled with non-zero values.
        /// </summary>
        /// <param name="count">The number of bytes to return.</param>
        /// <returns>A buffer filled with non-zero values.</returns>
        private static byte[] GetFilledBuffer(int count)
        {
            var buffer = new byte[count];
            ByteArray.Fill(buffer, 0, buffer.Length, 0xCC);
            return buffer;
        }

        /// <summary>
        /// Gets a buffer containing the specified byte array with padding.
        /// </summary>
        /// <param name="bytes">The byte array representing the value to test.</param>
        /// <param name="offset">The offset into the padded byte array to test.</param>
        /// <param name="padding">The number of bytes to pad.</param>
        /// <returns>A buffer containing the specified byte array with padding.</returns>
        private static byte[] GetPaddedBuffer(byte[] bytes, int offset, int padding)
        {
            var buffer = GetFilledBuffer(bytes.Length + padding);
            ByteArray.SetBytes(bytes, 0, buffer, offset, bytes.Length);
            return buffer;
        }

        /// <summary>
        /// Ensures that proper exceptions are thrown for invalid inputs.
        /// </summary>
        private void TestExceptions()
        {
            var value = default(T);

            if (this.getTestBytes != null)
            {
                var buffer = this.getTestBytes(value);

                Assert.Throws<ArgumentNullException>("buffer", () => this.getTestValue(null, 0));
                Assert.Throws<ArgumentOutOfRangeException>("offset", () => this.getTestValue(buffer, -1));
                Assert.Throws<ArgumentException>(() => this.getTestValue(buffer, 1));

                Assert.Throws<ArgumentNullException>("buffer", () => this.setTestValue(null, 0, value));
                Assert.Throws<ArgumentOutOfRangeException>("offset", () => this.setTestValue(buffer, -1, value));
                Assert.Throws<ArgumentException>(() => this.setTestValue(buffer, 1, value));
            }

            if (this.getBigEndianTestBytes != null)
            {
                var buffer = this.getBigEndianTestBytes(value, true);

                Assert.Throws<ArgumentNullException>("buffer", () => this.getBigEndianTestValue(null, 0, true));
                Assert.Throws<ArgumentOutOfRangeException>("offset", () => this.getBigEndianTestValue(buffer, -1, true));
                Assert.Throws<ArgumentException>(() => this.getBigEndianTestValue(buffer, 1, true));

                Assert.Throws<ArgumentNullException>("buffer", () => this.setBigEndianTestValue(null, 0, value, true));
                Assert.Throws<ArgumentOutOfRangeException>("offset", () => this.setBigEndianTestValue(buffer, -1, value, true));
                Assert.Throws<ArgumentException>(() => this.setBigEndianTestValue(buffer, 1, value, true));

                Assert.Throws<ArgumentNullException>("buffer", () => this.getBigEndianTestValue(null, 0, false));
                Assert.Throws<ArgumentOutOfRangeException>("offset", () => this.getBigEndianTestValue(buffer, -1, false));
                Assert.Throws<ArgumentException>(() => this.getBigEndianTestValue(buffer, 1, false));

                Assert.Throws<ArgumentNullException>("buffer", () => this.setBigEndianTestValue(null, 0, value, false));
                Assert.Throws<ArgumentOutOfRangeException>("offset", () => this.setBigEndianTestValue(buffer, -1, value, false));
                Assert.Throws<ArgumentException>(() => this.setBigEndianTestValue(buffer, 1, value, false));
            }
        }
    }
}