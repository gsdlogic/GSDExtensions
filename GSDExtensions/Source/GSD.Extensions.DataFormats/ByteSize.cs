// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteSize.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats;

using System;
using System.Globalization;

/// <summary>
/// Provides methods to calculate byte size strings.
/// </summary>
public static class ByteSize
{
    /// <summary>
    /// The number of bits in a byte.
    /// </summary>
    public const double Bit = 1.0 / 8.0;

    /// <summary>
    /// The number of bytes in a byte.
    /// </summary>
    public const double Byte = 1.0;

    /// <summary>
    /// The number of bytes in a exa byte.
    /// </summary>
    public const double ExaByte = 1024.0 * PetaByte;

    /// <summary>
    /// The number of bytes in a giga byte.
    /// </summary>
    public const double GigaByte = 1024.0 * MegaByte;

    /// <summary>
    /// The number of bytes in a kilo byte.
    /// </summary>
    public const double KiloByte = 1024.0 * Byte;

    /// <summary>
    /// The number of bytes in a mega byte.
    /// </summary>
    public const double MegaByte = 1024.0 * KiloByte;

    /// <summary>
    /// The number of bytes in a nibble.
    /// </summary>
    public const double Nibble = 0.5;

    /// <summary>
    /// The number of bytes in a peta byte.
    /// </summary>
    public const double PetaByte = 1024.0 * TeraByte;

    /// <summary>
    /// The number of bytes in a tera byte.
    /// </summary>
    public const double TeraByte = 1024.0 * GigaByte;

    /// <summary>
    /// The number of bytes in a yotta byte.
    /// </summary>
    public const double YottaByte = 1024.0 * ZettaByte;

    /// <summary>
    /// The number of bytes in a zetta byte.
    /// </summary>
    public const double ZettaByte = 1024.0 * ExaByte;

    /// <summary>
    /// Gets a string that represents the number of bytes.
    /// </summary>
    /// <param name="size">Total number of bytes.</param>
    /// <returns>A string that represents the number of bytes.</returns>
    public static string ToString(long size)
    {
        return ToString(CultureInfo.InvariantCulture, "{0} {1}", size);
    }

    /// <summary>
    /// Gets a string that represents the number of bytes.
    /// </summary>
    /// <param name="format">The format for the byte size.</param>
    /// <param name="size">Total number of bytes.</param>
    /// <returns>A string that represents the number of bytes.</returns>
    public static string ToString(string format, long size)
    {
        return ToString(CultureInfo.InvariantCulture, format, size);
    }

    /// <summary>
    /// Gets a string that represents the number of bytes.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format for the byte size.</param>
    /// <param name="size">Total number of bytes.</param>
    /// <returns>A string that represents the number of bytes.</returns>
    public static string ToString(IFormatProvider formatProvider, string format, long size)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        var order = 0;

        while ((size >= 1024) && (order < sizes.Length - 1))
        {
            order++;
            size /= 1024;
        }

        return string.Format(formatProvider, format, size, sizes[order]);
    }
}