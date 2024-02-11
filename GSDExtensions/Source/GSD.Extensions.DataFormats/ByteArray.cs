// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteArray.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats;

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GSD.Extensions.DataFormats.Properties;

/// <summary>
/// Provides methods to perform operations on byte arrays.
/// </summary>
public static class ByteArray
{
    /// <summary>
    /// Compares two byte arrays and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="left">The first byte array to compare.</param>
    /// <param name="right">The second byte array to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of <paramref name="left" /> and <paramref name="right" />.
    /// If <paramref name="left" /> is less than <paramref name="right" />, the return value will be less than zero.
    /// If <paramref name="left" /> is equal to <paramref name="right" />, the return value will be zero.
    /// If <paramref name="left" /> is greater than <paramref name="right" />, the return value will be greater than zero.
    /// </returns>
    public static int Compare(byte[] left, byte[] right)
    {
        if (left == null)
        {
            if (right == null)
            {
                return 0;
            }

            return -1;
        }

        if (right == null)
        {
            return 1;
        }

        var length = left.Length < right.Length ? left.Length : right.Length;

        for (var i = 0; i < length; i++)
        {
            if (left[i] != right[i])
            {
                return left[i] - right[i];
            }
        }

        return left.Length - right.Length;
    }

    /// <summary>
    /// Fills a byte array starting at a particular offset with the specified number of repeated bytes or pattern of bytes.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="count">The number of bytes to write.</param>
    /// <param name="value">The byte or pattern of bytes used to fill the byte array.</param>
    public static void Fill(byte[] buffer, int offset, int count, params byte[] value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var endIndex = offset + count;

        if (endIndex > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), nameof(count)));
        }

        if ((value == null) || (value.Length == 0))
        {
            for (var i = offset; i < endIndex; i++)
            {
                buffer[i] = 0x00;
            }
        }
        else
        {
            for (int i = offset, j = 0; i < endIndex; i++, j++)
            {
                if (j >= value.Length)
                {
                    j = 0;
                }

                buffer[i] = value[j];
            }
        }
    }

    /// <summary>
    /// Gets a 32-bit signed integer in compressed format from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="length">When the method completes, contains the number of bytes used to store the value in the byte array.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public static int Get7BitEncodedInt32(byte[] buffer, int offset, out int length)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var count = 0;
        var shift = 0;
        var index = offset;
        byte b;

        do
        {
            if (shift == 35)
            {
                throw new FormatException(Resources.FormatException_Invalid7BitEncodedInt32);
            }

            if (index + 1 > buffer.Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
            }

            b = buffer[index++];
            count |= (b & 0x7F) << shift;
            shift += 7;
        }
        while ((b & 0x80) != 0);

        length = index - offset;
        return count;
    }

    /// <summary>
    /// Gets a string from a byte array starting at a particular offset. The string is prefixed with the length encoded as an integer seven bits at a time.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="encoding">The <see cref="Encoding" /> used to store the string in the byte array.</param>
    /// <param name="length">When the method completes, contains the number of bytes used to store the string in the byte array, including the prefixed length.</param>
    /// <returns>A string value.</returns>
    public static string Get7BitLengthEncodedString(byte[] buffer, int offset, Encoding encoding, out int length)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        var count = Get7BitEncodedInt32(buffer, offset, out var countLength);

        length = countLength + count;
        return encoding.GetString(buffer, offset + countLength, count);
    }

    /// <summary>
    /// Gets a boolean value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <returns>A boolean value.</returns>
    public static bool GetBoolean(byte[] buffer, int offset)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 1 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        return buffer[offset] != 0x00;
    }

    /// <summary>
    /// Gets an unsigned byte from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <returns>An unsigned byte.</returns>
    public static byte GetByte(byte[] buffer, int offset)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 1 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        return buffer[offset];
    }

    /// <summary>
    /// Gets a byte array representing a boolean value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>A byte array with a length of 1.</returns>
    public static byte[] GetBytes(bool value)
    {
        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a Unicode character.
    /// </summary>
    /// <param name="value">The Unicode character.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 2.</returns>
    public static byte[] GetBytes(char value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)(((short)value >> 8) & 0xFF),
                (byte)((short)value & 0xFF),
            };
        }

        return new[]
        {
            (byte)((short)value & 0xFF),
            (byte)(((short)value >> 8) & 0xFF),
        };
    }

    /// <summary>
    /// Gets a byte array representing an unsigned byte.
    /// </summary>
    /// <param name="value">The unsigned byte.</param>
    /// <returns>A byte array with a length of 1.</returns>
    public static byte[] GetBytes(byte value)
    {
        return new[] { value };
    }

    /// <summary>
    /// Gets a byte array representing a signed byte.
    /// </summary>
    /// <param name="value">The signed byte.</param>
    /// <returns>A byte array with a length of 1.</returns>
    [CLSCompliant(false)]
    public static byte[] GetBytes(sbyte value)
    {
        return new[] { (byte)value };
    }

    /// <summary>
    /// Gets a byte array representing a 16-bit signed integer.
    /// </summary>
    /// <param name="value">The 16-bit signed integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 2.</returns>
    public static byte[] GetBytes(short value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            };
        }

        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a 32-bit signed integer.
    /// </summary>
    /// <param name="value">The 32-bit signed integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 4.</returns>
    public static byte[] GetBytes(int value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            };
        }

        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a 64-bit signed integer.
    /// </summary>
    /// <param name="value">The 64-bit signed integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 8.</returns>
    public static byte[] GetBytes(long value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)((value >> 56) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            };
        }

        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a 16-bit unsigned integer.
    /// </summary>
    /// <param name="value">The 16-bit unsigned integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 2.</returns>
    [CLSCompliant(false)]
    public static byte[] GetBytes(ushort value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            };
        }

        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a 32-bit unsigned integer.
    /// </summary>
    /// <param name="value">The 32-bit unsigned integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 4.</returns>
    [CLSCompliant(false)]
    public static byte[] GetBytes(uint value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            };
        }

        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a 64-bit unsigned integer.
    /// </summary>
    /// <param name="value">The 64-bit unsigned integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 8.</returns>
    [CLSCompliant(false)]
    public static byte[] GetBytes(ulong value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return new[]
            {
                (byte)((value >> 56) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF),
            };
        }

        return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets a byte array representing a single-precision floating point value.
    /// </summary>
    /// <param name="value">The single-precision floating point value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 4.</returns>
    public static unsafe byte[] GetBytes(float value, bool isBigEndian = false)
    {
        return GetBytes(*(int*)&value, isBigEndian);
    }

    /// <summary>
    /// Gets a byte array representing a double-precision floating point value.
    /// </summary>
    /// <param name="value">The double-precision floating point value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 8.</returns>
    public static unsafe byte[] GetBytes(double value, bool isBigEndian = false)
    {
        return GetBytes(*(long*)&value, isBigEndian);
    }

    /// <summary>
    /// Gets a byte array representing a decimal value.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 16.</returns>
    public static unsafe byte[] GetBytes(decimal value, bool isBigEndian = false)
    {
        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            var buffer = new byte[16];
            var c = (byte*)&value;

            buffer[0] = c[15];
            buffer[1] = c[14];
            buffer[2] = c[13];
            buffer[3] = c[12];
            buffer[4] = c[11];
            buffer[5] = c[10];
            buffer[6] = c[9];
            buffer[7] = c[8];
            buffer[8] = c[7];
            buffer[9] = c[6];
            buffer[10] = c[5];
            buffer[11] = c[4];
            buffer[12] = c[3];
            buffer[13] = c[2];
            buffer[14] = c[1];
            buffer[15] = c[0];

            return buffer;
        }

        var bytes = new byte[16];

        fixed (byte* b = bytes)
        {
            *(decimal*)b = value;
        }

        return bytes;
    }

    /// <summary>
    /// Gets a byte array representing a date/time value.
    /// </summary>
    /// <param name="value">The date/time value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A byte array with a length of 8.</returns>
    public static unsafe byte[] GetBytes(DateTime value, bool isBigEndian = false)
    {
        return GetBytes(*(long*)&value, isBigEndian);
    }

    /// <summary>
    /// Gets a byte array representing a globally unique identifier (GUID).
    /// </summary>
    /// <param name="value">The globally unique identifier (GUID).</param>
    /// <returns>A byte array with a length of 16.</returns>
    public static byte[] GetBytes(Guid value)
    {
        return value.ToByteArray();
    }

    /// <summary>
    /// Gets a byte array representing an object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <returns>A byte array representing the object.</returns>
    public static byte[] GetBytes(object value)
    {
        var length = Marshal.SizeOf(value);
        var buffer = new byte[length];

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var address = handle.AddrOfPinnedObject();

        try
        {
            Marshal.StructureToPtr(value, address, false);
        }
        finally
        {
            handle.Free();
        }

        return buffer;
    }

    /// <summary>
    /// Gets a byte array from a source byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The source byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A new byte array.</returns>
    public static byte[] GetBytes(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        var bytes = new byte[count];
        Buffer.BlockCopy(buffer, offset, bytes, 0, count);
        return bytes;
    }

    /// <summary>
    /// Gets an Unicode character from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A Unicode character.</returns>
    public static char GetChar(byte[] buffer, int offset, bool isBigEndian = false)
    {
        return (char)GetInt16(buffer, offset, isBigEndian);
    }

    /// <summary>
    /// Gets a date/time value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A date/time value.</returns>
    public static unsafe DateTime GetDateTime(byte[] buffer, int offset, bool isBigEndian = false)
    {
        var value = GetInt64(buffer, offset, isBigEndian);
        return *(DateTime*)&value;
    }

    /// <summary>
    /// Gets a decimal value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A decimal value.</returns>
    public static unsafe decimal GetDecimal(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 16 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        decimal value;
        var c = (byte*)&value;

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            c[0] = buffer[offset + 15];
            c[1] = buffer[offset + 14];
            c[2] = buffer[offset + 13];
            c[3] = buffer[offset + 12];
            c[4] = buffer[offset + 11];
            c[5] = buffer[offset + 10];
            c[6] = buffer[offset + 9];
            c[7] = buffer[offset + 8];
            c[8] = buffer[offset + 7];
            c[9] = buffer[offset + 6];
            c[10] = buffer[offset + 5];
            c[11] = buffer[offset + 4];
            c[12] = buffer[offset + 3];
            c[13] = buffer[offset + 2];
            c[14] = buffer[offset + 1];
            c[15] = buffer[offset];
        }
        else
        {
            c[0] = buffer[offset];
            c[1] = buffer[offset + 1];
            c[2] = buffer[offset + 2];
            c[3] = buffer[offset + 3];
            c[4] = buffer[offset + 4];
            c[5] = buffer[offset + 5];
            c[6] = buffer[offset + 6];
            c[7] = buffer[offset + 7];
            c[8] = buffer[offset + 8];
            c[9] = buffer[offset + 9];
            c[10] = buffer[offset + 10];
            c[11] = buffer[offset + 11];
            c[12] = buffer[offset + 12];
            c[13] = buffer[offset + 13];
            c[14] = buffer[offset + 14];
            c[15] = buffer[offset + 15];
        }

        return value;
    }

    /// <summary>
    /// Gets a double precision floating point value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A double precision floating point value.</returns>
    public static unsafe double GetDouble(byte[] buffer, int offset, bool isBigEndian = false)
    {
        var value = GetInt64(buffer, offset, isBigEndian);
        return *(double*)&value;
    }

    /// <summary>
    /// Gets globally unique identifier from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <returns>A globally unique identifier.</returns>
    public static Guid GetGuid(byte[] buffer, int offset)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 16 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        var bytes = new byte[16];
        Buffer.BlockCopy(buffer, offset, bytes, 0, 16);
        return new Guid(bytes);
    }

    /// <summary>
    /// Gets a string representing a table containing three columns for the offset, hexadecimal and ASCII representation of a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A string representing a table containing three columns for the offset, hexadecimal and ASCII representation of the specified range of bytes from the byte array.</returns>
    public static string GetHexDump(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), nameof(count)));
        }

        var offsetCount = offset + count;
        var lineOffset = (offset / 16) * 16;
        var lineOffsetCount = ((offsetCount + 15) / 16) * 16;

        var result = new StringBuilder();
        var hex = new StringBuilder(52);
        var ascii = new StringBuilder(16);

        for (var i = lineOffset; i < lineOffsetCount; i++)
        {
            if (i % 8 == 0)
            {
                hex.Append(' ');
            }

            if ((i < offset) || (i >= offsetCount))
            {
                hex.Append("   ");
                ascii.Append(' ');
            }
            else
            {
                hex.Append(' ');
                hex.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
                ascii.Append((buffer[i] < 32) || (buffer[i] > 126) ? '.' : (char)buffer[i]);
            }

            if (i % 16 == 15)
            {
                result.Append(lineOffset.ToString("X8", CultureInfo.InvariantCulture));
                result.Append(hex);
                result.Append("  ");
                result.Append(ascii);
                result.AppendLine();

                hex.Clear();
                ascii.Clear();
                lineOffset += 16;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Gets a string representation of a byte array encoded as hexadecimal digits.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A string representation of the byte array encoded as hexadecimal digits.</returns>
    public static string GetHexString(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), nameof(count)));
        }

        if (count > int.MaxValue / 2)
        {
            throw new ArgumentOutOfRangeException(nameof(count), Resources.ArgumentOutOfRangeException_CountIsTooBig);
        }

        var offsetCount = offset + count;
        var hex = new StringBuilder(count * 2);

        for (var i = offset; i < offsetCount; i++)
        {
            hex.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
        }

        return hex.ToString();
    }

    /// <summary>
    /// Gets a 16-bit signed integer from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A 16-bit signed integer.</returns>
    public static short GetInt16(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 2 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return (short)(
                (buffer[offset] << 8) |
                buffer[offset + 1]);
        }

        return (short)(
            (buffer[offset + 1] << 8) |
            buffer[offset]);
    }

    /// <summary>
    /// Gets a 32-bit signed integer from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public static int GetInt32(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 4 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return (buffer[offset] << 24) |
                   (buffer[offset + 1] << 16) |
                   (buffer[offset + 2] << 8) |
                   buffer[offset + 3];
        }

        return (buffer[offset + 3] << 24) |
               (buffer[offset + 2] << 16) |
               (buffer[offset + 1] << 8) |
               buffer[offset];
    }

    /// <summary>
    /// Gets a 64-bit signed integer from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public static long GetInt64(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 8 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return ((long)(
                       (buffer[offset] << 24) |
                       (buffer[offset + 1] << 16) |
                       (buffer[offset + 2] << 8) |
                       buffer[offset + 3]) << 32) |
                   (uint)(
                       (buffer[offset + 4] << 24) |
                       (buffer[offset + 5] << 16) |
                       (buffer[offset + 6] << 8) |
                       buffer[offset + 7]);
        }

        return ((long)(
                   (buffer[offset + 7] << 24) |
                   (buffer[offset + 6] << 16) |
                   (buffer[offset + 5] << 8) |
                   buffer[offset + 4]) << 32) |
               (uint)(
                   (buffer[offset + 3] << 24) |
                   (buffer[offset + 2] << 16) |
                   (buffer[offset + 1] << 8) |
                   buffer[offset]);
    }

    /// <summary>
    /// Gets a string from a byte array starting at a particular offset. The string is terminated with a null ('\0') character.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="encoding">The <see cref="Encoding" /> used to store the string in the byte array.</param>
    /// <param name="length">When the method completes, contains the number of bytes used to store the string in the byte array, including the null ('\0') character.</param>
    /// <returns>A string value.</returns>
    public static string GetNullTerminatedString(byte[] buffer, int offset, Encoding encoding, out int length)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var byteCount = encoding is UnicodeEncoding ? 2 : 1;
        var index = IndexOf(buffer, offset, byteCount, new byte[byteCount]);
        var count = index - offset;

        length = count + byteCount;
        return encoding.GetString(buffer, offset, count);
    }

    /// <summary>
    /// Gets an object from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="type">The type of object to return.</param>
    /// <returns>The object that was read.</returns>
    public static object GetObject(byte[] buffer, int offset, Type type)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        var length = Marshal.SizeOf(type);

        if (length > buffer.Length)
        {
            return null;
        }

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var address = handle.AddrOfPinnedObject() + offset;
        object result;

        try
        {
            result = Marshal.PtrToStructure(address, type);
        }
        finally
        {
            handle.Free();
        }

        return result;
    }

    /// <summary>
    /// Gets an object from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <returns>The object that was read.</returns>
    public static T GetObject<T>(byte[] buffer, int offset)
    {
        return (T)GetObject(buffer, offset, typeof(T));
    }

    /// <summary>
    /// Gets a signed byte from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <returns>A signed byte.</returns>
    [CLSCompliant(false)]
    public static sbyte GetSByte(byte[] buffer, int offset)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 1 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        return (sbyte)buffer[offset];
    }

    /// <summary>
    /// Gets a single precision floating point value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A single precision floating point value.</returns>
    public static unsafe float GetSingle(byte[] buffer, int offset, bool isBigEndian = false)
    {
        var value = GetInt32(buffer, offset, isBigEndian);
        return *(float*)&value;
    }

    /// <summary>
    /// Gets a string from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <param name="encoding">The <see cref="Encoding" /> used to store the string in the byte array.</param>
    /// <returns>A string value.</returns>
    public static string GetString(byte[] buffer, int offset, int count, Encoding encoding)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        return encoding.GetString(buffer, offset, count).TrimEnd('\0');
    }

    /// <summary>
    /// Gets a 16-bit unsigned integer from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A 16-bit unsigned integer.</returns>
    [CLSCompliant(false)]
    public static ushort GetUInt16(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 2 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return (ushort)(
                (buffer[offset] << 8) |
                buffer[offset + 1]);
        }

        return (ushort)(
            (buffer[offset + 1] << 8) |
            buffer[offset]);
    }

    /// <summary>
    /// Gets a 32-bit unsigned integer from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A 32-bit unsigned integer.</returns>
    [CLSCompliant(false)]
    public static uint GetUInt32(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 4 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return (uint)(
                (buffer[offset] << 24) |
                (buffer[offset + 1] << 16) |
                (buffer[offset + 2] << 8) |
                buffer[offset + 3]);
        }

        return (uint)(
            (buffer[offset + 3] << 24) |
            (buffer[offset + 2] << 16) |
            (buffer[offset + 1] << 8) |
            buffer[offset]);
    }

    /// <summary>
    /// Gets a 64-bit unsigned integer from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    /// <returns>A 64-bit unsigned integer.</returns>
    [CLSCompliant(false)]
    public static ulong GetUInt64(byte[] buffer, int offset, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 8 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            return ((ulong)(
                       (buffer[offset] << 24) |
                       (buffer[offset + 1] << 16) |
                       (buffer[offset + 2] << 8) |
                       buffer[offset + 3]) << 32) |
                   (uint)(
                       (buffer[offset + 4] << 24) |
                       (buffer[offset + 5] << 16) |
                       (buffer[offset + 6] << 8) |
                       buffer[offset + 7]);
        }

        return ((ulong)(
                   (buffer[offset + 7] << 24) |
                   (buffer[offset + 6] << 16) |
                   (buffer[offset + 5] << 8) |
                   buffer[offset + 4]) << 32) |
               (uint)(
                   (buffer[offset + 3] << 24) |
                   (buffer[offset + 2] << 16) |
                   (buffer[offset + 1] << 8) |
                   buffer[offset]);
    }

    /// <summary>
    /// Gets the zero-based index of the first occurrence of a specified byte or pattern of bytes within a byte array.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="step">The amount by which the counter is incremented each time through the loop.</param>
    /// <param name="value">The byte or pattern of bytes to seek.</param>
    /// <returns>The zero-based index of <paramref name="value" /> from the start of the array if the byte or patter of bytes is found, or -1 if it is not.</returns>
    public static int IndexOf(byte[] buffer, int offset, int step, params byte[] value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (step < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(step), Resources.ArgumentOutOfRangeException_StepMustBeGreaterThanZero);
        }

        if ((value == null) || (value.Length == 0))
        {
            return -1;
        }

        while (offset + value.Length <= buffer.Length)
        {
            if (buffer[offset] == value[0])
            {
                var match = true;

                for (var i = 1; i < value.Length; i++)
                {
                    if (buffer[offset + i] != value[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return offset;
                }
            }

            offset += step;
        }

        return -1;
    }

    /// <summary>
    /// Gets the zero-based index of the first occurrence of any byte in a specified byte or array of bytes within a byte array.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="value">The byte or array of bytes to seek.</param>
    /// <returns>The zero-based index of the first occurrence where any byte in <paramref name="value" /> was found, or -1 if no byte was found.</returns>
    public static int IndexOfAny(byte[] buffer, int offset, params byte[] value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if ((value == null) || (value.Length == 0))
        {
            value = new byte[1];
        }

        for (var i = offset; i < buffer.Length; i++)
        {
            if (value.Any(j => buffer[i] == j))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Stores a 32-bit signed integer in compressed format in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 32-bit signed integer.</param>
    /// <returns>The number of bytes used to store the value in the byte array.</returns>
    public static int Set7BitEncodedInt32(byte[] buffer, int offset, int value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var v = (uint)value;
        var index = offset;

        while (v >= 0x80)
        {
            if (index >= buffer.Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
            }

            buffer[index++] = (byte)(v | 0x80);
            v >>= 7;
        }

        if (index >= buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        buffer[index++] = (byte)v;
        return index - offset;
    }

    /// <summary>
    /// Stores a string in a byte array starting at a particular offset. The string is prefixed with the length encoded as an integer seven bits at a time.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The string value.</param>
    /// <param name="encoding">The <see cref="Encoding" /> used to store the string in the byte array.</param>
    /// <returns>The number of bytes used to store the string in the byte array, including the prefixed length.</returns>
    public static int Set7BitLengthEncodedString(byte[] buffer, int offset, string value, Encoding encoding)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.InvariantCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var bytes = encoding.GetBytes(value);

        if (offset + bytes.Length >= buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        var countLength = Set7BitEncodedInt32(buffer, offset, bytes.Length);

        var length = countLength + bytes.Length;

        if (offset + length > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        Buffer.BlockCopy(bytes, 0, buffer, offset + countLength, bytes.Length);
        return length;
    }

    /// <summary>
    /// Stores a boolean value in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The boolean value.</param>
    public static void SetBoolean(byte[] buffer, int offset, bool value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 1 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        buffer[offset] = value ? (byte)0x01 : (byte)0x00;
    }

    /// <summary>
    /// Stores an unsigned byte in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The unsigned byte.</param>
    public static void SetByte(byte[] buffer, int offset, byte value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 1 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        buffer[offset] = value;
    }

    /// <summary>
    /// Stores a specified number of bytes from a source byte array starting at a particular offset in a destination byte array starting at a particular offset.
    /// </summary>
    /// <param name="sourceBuffer">The source byte array.</param>
    /// <param name="sourceOffset">The zero-based offset into <paramref name="sourceBuffer" /> at which to start reading.</param>
    /// <param name="destinationBuffer">The destination byte array.</param>
    /// <param name="destinationOffset">The zero-based offset into <paramref name="destinationBuffer" /> at which to start writing.</param>
    /// <param name="count">The number of bytes to write.</param>
    public static void SetBytes(byte[] sourceBuffer, int sourceOffset, byte[] destinationBuffer, int destinationOffset, int count)
    {
        Buffer.BlockCopy(sourceBuffer, sourceOffset, destinationBuffer, destinationOffset, count);
    }

    /// <summary>
    /// Stores an Unicode character in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The Unicode character.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static void SetChar(byte[] buffer, int offset, char value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 2 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)(((short)value >> 8) & 0xFF);
            buffer[offset + 1] = (byte)((short)value & 0xFF);
        }
        else
        {
            buffer[offset + 1] = (byte)(((short)value >> 8) & 0xFF);
            buffer[offset] = (byte)((short)value & 0xFF);
        }
    }

    /// <summary>
    /// Stores a date/time value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The date/time value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static unsafe void SetDateTime(byte[] buffer, int offset, DateTime value, bool isBigEndian = false)
    {
        SetInt64(buffer, offset, *(long*)&value, isBigEndian);
    }

    /// <summary>
    /// Stores a decimal value from a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The decimal value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static unsafe void SetDecimal(byte[] buffer, int offset, decimal value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 16 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        var c = (byte*)&value;

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = c[15];
            buffer[offset + 1] = c[14];
            buffer[offset + 2] = c[13];
            buffer[offset + 3] = c[12];
            buffer[offset + 4] = c[11];
            buffer[offset + 5] = c[10];
            buffer[offset + 6] = c[9];
            buffer[offset + 7] = c[8];
            buffer[offset + 8] = c[7];
            buffer[offset + 9] = c[6];
            buffer[offset + 10] = c[5];
            buffer[offset + 11] = c[4];
            buffer[offset + 12] = c[3];
            buffer[offset + 13] = c[2];
            buffer[offset + 14] = c[1];
            buffer[offset + 15] = c[0];
        }
        else
        {
            buffer[offset + 15] = c[15];
            buffer[offset + 14] = c[14];
            buffer[offset + 13] = c[13];
            buffer[offset + 12] = c[12];
            buffer[offset + 11] = c[11];
            buffer[offset + 10] = c[10];
            buffer[offset + 9] = c[9];
            buffer[offset + 8] = c[8];
            buffer[offset + 7] = c[7];
            buffer[offset + 6] = c[6];
            buffer[offset + 5] = c[5];
            buffer[offset + 4] = c[4];
            buffer[offset + 3] = c[3];
            buffer[offset + 2] = c[2];
            buffer[offset + 1] = c[1];
            buffer[offset] = c[0];
        }
    }

    /// <summary>
    /// Stores a double precision floating point value in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The double precision floating point value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static unsafe void SetDouble(byte[] buffer, int offset, double value, bool isBigEndian = false)
    {
        SetInt64(buffer, offset, *(long*)&value, isBigEndian);
    }

    /// <summary>
    /// Stores a globally unique identifier in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The globally unique identifier.</param>
    public static void SetGuid(byte[] buffer, int offset, Guid value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 16 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        var bytes = value.ToByteArray();
        Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
    }

    /// <summary>
    /// Stores a 16-bit signed integer in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 16-bit signed integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static void SetInt16(byte[] buffer, int offset, short value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 2 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 1] = (byte)(value & 0xFF);
        }
        else
        {
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset] = (byte)(value & 0xFF);
        }
    }

    /// <summary>
    /// Stores a 32-bit signed integer in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 32-bit signed integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static void SetInt32(byte[] buffer, int offset, int value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 4 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 3] = (byte)(value & 0xFF);
        }
        else
        {
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset] = (byte)(value & 0xFF);
        }
    }

    /// <summary>
    /// Stores a 64-bit signed integer in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 64-bit signed integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static void SetInt64(byte[] buffer, int offset, long value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 8 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)((value >> 56) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 48) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 40) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 32) & 0xFF);
            buffer[offset + 4] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 5] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 6] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 7] = (byte)(value & 0xFF);
        }
        else
        {
            buffer[offset + 7] = (byte)((value >> 56) & 0xFF);
            buffer[offset + 6] = (byte)((value >> 48) & 0xFF);
            buffer[offset + 5] = (byte)((value >> 40) & 0xFF);
            buffer[offset + 4] = (byte)((value >> 32) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset] = (byte)(value & 0xFF);
        }
    }

    /// <summary>
    /// Stores a string in a byte array starting at a particular offset. The string is terminated with a null ('\0') character.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The string value.</param>
    /// <param name="encoding">The <see cref="Encoding" /> used to store the string in the byte array.</param>
    /// <returns>The number of bytes used to store the string in the byte array, including the null ('\0') character.</returns>
    public static int SetNullTerminatedString(byte[] buffer, int offset, string value, Encoding encoding)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var bytes = encoding.GetBytes(value);
        var isMultiByte = encoding is UnicodeEncoding;
        var count = bytes.Length + (isMultiByte ? 2 : 1);

        if (offset + count > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
        buffer[offset + bytes.Length] = 0x00;

        if (isMultiByte)
        {
            buffer[offset + bytes.Length + 1] = 0x00;
        }

        return count;
    }

    /// <summary>
    /// Stores an object in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The object to store.</param>
    /// <returns>The number of bytes used to store the object in the byte array.</returns>
    public static int SetObject(byte[] buffer, int offset, object value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        var length = Marshal.SizeOf(value);

        if (offset + length > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var address = handle.AddrOfPinnedObject() + offset;

        try
        {
            Marshal.StructureToPtr(value, address, false);
        }
        finally
        {
            handle.Free();
        }

        return length;
    }

    /// <summary>
    /// Stores a signed byte in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The signed byte.</param>
    [CLSCompliant(false)]
    public static void SetSByte(byte[] buffer, int offset, sbyte value)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 1 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        buffer[offset] = (byte)value;
    }

    /// <summary>
    /// Stores a single precision floating point value in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start reading.</param>
    /// <param name="value">The single precision floating point value.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    public static unsafe void SetSingle(byte[] buffer, int offset, float value, bool isBigEndian = false)
    {
        SetInt32(buffer, offset, *(int*)&value, isBigEndian);
    }

    /// <summary>
    /// Stores a string in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The string value.</param>
    /// <param name="encoding">The <see cref="Encoding" /> used to store the string in the byte array.</param>
    /// <returns>The number of bytes used to store the string in the byte array.</returns>
    public static int SetString(byte[] buffer, int offset, string value, Encoding encoding)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        return encoding.GetBytes(value, 0, value.Length, buffer, offset);
    }

    /// <summary>
    /// Stores a 16-bit unsigned integer in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 16-bit unsigned integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    [CLSCompliant(false)]
    public static void SetUInt16(byte[] buffer, int offset, ushort value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 2 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 1] = (byte)(value & 0xFF);
        }
        else
        {
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset] = (byte)(value & 0xFF);
        }
    }

    /// <summary>
    /// Stores a 32-bit unsigned integer in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 32-bit unsigned integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    [CLSCompliant(false)]
    public static void SetUInt32(byte[] buffer, int offset, uint value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 4 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 3] = (byte)(value & 0xFF);
        }
        else
        {
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset] = (byte)(value & 0xFF);
        }
    }

    /// <summary>
    /// Stores a 64-bit unsigned integer in a byte array starting at a particular offset.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <param name="offset">The zero-based offset into <paramref name="buffer" /> at which to start writing.</param>
    /// <param name="value">The 64-bit unsigned integer.</param>
    /// <param name="isBigEndian">Indicates whether the value is stored in the the byte array as most significant byte first.</param>
    [CLSCompliant(false)]
    public static void SetUInt64(byte[] buffer, int offset, ulong value, bool isBigEndian = false)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), string.Format(CultureInfo.CurrentCulture, Resources.ArgumentOutOfRangeException_NonNegativeRequired_Parameter, nameof(offset)));
        }

        if (offset + 8 > buffer.Length)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentException_Invalid_Offset_Length, nameof(offset), "data length"));
        }

        if (isBigEndian == BitConverter.IsLittleEndian)
        {
            buffer[offset] = (byte)((value >> 56) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 48) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 40) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 32) & 0xFF);
            buffer[offset + 4] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 5] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 6] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 7] = (byte)(value & 0xFF);
        }
        else
        {
            buffer[offset + 7] = (byte)((value >> 56) & 0xFF);
            buffer[offset + 6] = (byte)((value >> 48) & 0xFF);
            buffer[offset + 5] = (byte)((value >> 40) & 0xFF);
            buffer[offset + 4] = (byte)((value >> 32) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset] = (byte)(value & 0xFF);
        }
    }
}