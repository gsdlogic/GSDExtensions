// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base32.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats;

using System;
using System.Text;

/// <summary>
/// Provides and implementation of the Base32 encoding and decoding (see http://tools.ietf.org/html/rfc3548#section-5).
/// </summary>
/// <remarks>
/// From algorithm made internal by Microsoft: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/Base32.cs.
/// </remarks>
public static class Base32
{
    /// <summary>
    /// The set of Base32 characters.
    /// </summary>
    private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    /// <summary>
    /// Converts the specified string, which encodes binary data as Base32 digits, to an equivalent 8-bit unsigned integer array.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="input" />.</returns>
    public static byte[] FromBase32String(string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        input = input.TrimEnd('=').ToUpperInvariant();

        if (input.Length == 0)
        {
            return Array.Empty<byte>();
        }

        var output = new byte[(input.Length * 5) / 8];
        var bitIndex = 0;
        var inputIndex = 0;
        var outputBits = 0;
        var outputIndex = 0;

        while (outputIndex < output.Length)
        {
            var byteIndex = Base32Chars.IndexOf(input[inputIndex], StringComparison.Ordinal);

            if (byteIndex < 0)
            {
                throw new FormatException();
            }

            var bits = Math.Min(5 - bitIndex, 8 - outputBits);
            output[outputIndex] <<= bits;
            output[outputIndex] |= (byte)(byteIndex >> (5 - (bitIndex + bits)));

            bitIndex += bits;

            if (bitIndex >= 5)
            {
                inputIndex++;
                bitIndex = 0;
            }

            outputBits += bits;

            if (outputBits < 8)
            {
                continue;
            }

            outputIndex++;
            outputBits = 0;
        }

        return output;
    }

    /// <summary>
    /// Converts the value of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with Base32 digits.
    /// </summary>
    /// <param name="input">An array of 8-bit unsigned integers.</param>
    /// <returns>The string representation, in Base32, of the contents of <paramref name="input" />.</returns>
    public static string ToBase32String(byte[] input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        var sb = new StringBuilder();

        for (var offset = 0; offset < input.Length;)
        {
            var numCharsToOutput = GetNextGroup(
                input,
                ref offset,
                out var a,
                out var b,
                out var c,
                out var d,
                out var e,
                out var f,
                out var g,
                out var h);

            sb.Append(numCharsToOutput >= 1 ? Base32Chars[a] : '=');
            sb.Append(numCharsToOutput >= 2 ? Base32Chars[b] : '=');
            sb.Append(numCharsToOutput >= 3 ? Base32Chars[c] : '=');
            sb.Append(numCharsToOutput >= 4 ? Base32Chars[d] : '=');
            sb.Append(numCharsToOutput >= 5 ? Base32Chars[e] : '=');
            sb.Append(numCharsToOutput >= 6 ? Base32Chars[f] : '=');
            sb.Append(numCharsToOutput >= 7 ? Base32Chars[g] : '=');
            sb.Append(numCharsToOutput >= 8 ? Base32Chars[h] : '=');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets the next group of bytes.
    /// </summary>
    /// <param name="input">The input byte array.</param>
    /// <param name="offset">The zero-base offset into the byte array.</param>
    /// <param name="a">The first output byte.</param>
    /// <param name="b">The second output byte.</param>
    /// <param name="c">The third output byte.</param>
    /// <param name="d">The fourth output byte.</param>
    /// <param name="e">The fifth output byte.</param>
    /// <param name="f">The sixth output byte.</param>
    /// <param name="g">The seventh output byte.</param>
    /// <param name="h">The eighth output byte.</param>
    /// <returns>The number of bytes that were output.</returns>
    private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
    {
        var retVal = (input.Length - offset) switch
        {
            1 => 2,
            2 => 4,
            3 => 5,
            4 => 7,
            _ => 8,
        };

        var b1 = offset < input.Length ? input[offset++] : 0U;
        var b2 = offset < input.Length ? input[offset++] : 0U;
        var b3 = offset < input.Length ? input[offset++] : 0U;
        var b4 = offset < input.Length ? input[offset++] : 0U;
        var b5 = offset < input.Length ? input[offset++] : 0U;

        a = (byte)(b1 >> 3);
        b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
        c = (byte)((b2 >> 1) & 0x1f);
        d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
        e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
        f = (byte)((b4 >> 2) & 0x1f);
        g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
        h = (byte)(b5 & 0x1f);

        return retVal;
    }
}