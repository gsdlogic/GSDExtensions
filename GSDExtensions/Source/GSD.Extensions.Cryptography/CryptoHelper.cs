// <copyright file="CryptoHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;
using GSD.Extensions.Cryptography.Properties;

/// <summary>
/// Provides cryptographic utility methods.
/// </summary>
public static class CryptoHelper
{
    /// <summary>
    /// Generates a cryptographically secure byte array of the specified size.
    /// </summary>
    /// <param name="size">The size of the byte array to generate.</param>
    /// <returns>A cryptographically secure byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="size" /> is less than or equal to zero.</exception>
    public static byte[] GenerateRandomBytes(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException(Resources.ArgumentException_SizeMustBeGreaterThanZero, nameof(size));
        }

        using var rng = new RNGCryptoServiceProvider();
        var bytes = new byte[size];
        rng.GetBytes(bytes);
        return bytes;
    }

    /// <summary>
    /// Generates a random GUID using a secure random number generator.
    /// </summary>
    /// <returns>A new randomly generated GUID.</returns>
    public static Guid GenerateRandomGuid()
    {
        var bytes = GenerateRandomBytes(16);
        bytes[8] = (byte)((bytes[8] & 0xBF) | 0x80); // Set the version to 4 (UUID)
        bytes[7] = (byte)((bytes[7] & 0x4F) | 0x40); // Set the variant to RFC 4122
        return new Guid(bytes);
    }

    /// <summary>
    /// Compares two byte arrays in a time-constant manner to prevent timing attacks.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>True if the byte arrays are equal; otherwise, false.</returns>
    public static bool SlowEquals(byte[] a, byte[] b)
    {
        if ((a == null) || (b == null) || (a.Length != b.Length))
        {
            return false;
        }

        var result = 0;

        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}