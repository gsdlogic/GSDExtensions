// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitSize.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats;

using System;
using System.Globalization;

/// <summary>
/// Provides methods to calculate bit size strings.
/// </summary>
public static class BitSize
{
    /// <summary>
    /// The number of bits in a bit.
    /// </summary>
    public const double Bit = 1.0;

    /// <summary>
    /// The number of bits in a bit.
    /// </summary>
    public const double Byte = 8.0;

    /// <summary>
    /// The number of bits in a exa bit.
    /// </summary>
    public const double ExaBit = 1024.0 * PetaBit;

    /// <summary>
    /// The number of bits in a giga bit.
    /// </summary>
    public const double GigaBit = 1024.0 * MegaBit;

    /// <summary>
    /// The number of bits in a kilo bit.
    /// </summary>
    public const double KiloBit = 1024.0 * Bit;

    /// <summary>
    /// The number of bits in a mega bit.
    /// </summary>
    public const double MegaBit = 1024.0 * KiloBit;

    /// <summary>
    /// The number of bits in a nibble.
    /// </summary>
    public const double Nibble = 4.0;

    /// <summary>
    /// The number of bits in a peta bit.
    /// </summary>
    public const double PetaBit = 1024.0 * TeraBit;

    /// <summary>
    /// The number of bits in a tera bit.
    /// </summary>
    public const double TeraBit = 1024.0 * GigaBit;

    /// <summary>
    /// The number of bits in a yotta bit.
    /// </summary>
    public const double YottaBit = 1024.0 * ZettaBit;

    /// <summary>
    /// The number of bits in a zetta bit.
    /// </summary>
    public const double ZettaBit = 1024.0 * ExaBit;

    /// <summary>
    /// Gets a string that represents the number of bits.
    /// </summary>
    /// <param name="size">Total number of bits.</param>
    /// <returns>A string that represents the number of bits.</returns>
    public static string ToString(long size)
    {
        return ToString(CultureInfo.InvariantCulture, "{0} {1}", size);
    }

    /// <summary>
    /// Gets a string that represents the number of bits.
    /// </summary>
    /// <param name="format">The format for the bit size.</param>
    /// <param name="size">Total number of bits.</param>
    /// <returns>A string that represents the number of bits.</returns>
    public static string ToString(string format, long size)
    {
        return ToString(CultureInfo.InvariantCulture, format, size);
    }

    /// <summary>
    /// Gets a string that represents the number of bits.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format for the bit size.</param>
    /// <param name="size">Total number of bits.</param>
    /// <returns>A string that represents the number of bits.</returns>
    public static string ToString(IFormatProvider formatProvider, string format, long size)
    {
        string[] sizes = { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb" };
        var order = 0;

        while ((size >= 1024) && (order < sizes.Length - 1))
        {
            order++;
            size /= 1024;
        }

        return string.Format(formatProvider, format, size, sizes[order]);
    }
}