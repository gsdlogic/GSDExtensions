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
    /// Computes an HMAC using SHA-256.
    /// </summary>
    /// <param name="key">The key used for HMAC.</param>
    /// <param name="data">The data to hash.</param>
    /// <returns>The HMAC-SHA-256 hash as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key" /> or <paramref name="data" /> is null.</exception>
    public static byte[] ComputeHmacSha256(byte[] key, byte[] data)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using var hmacSha256 = new HMACSHA256(key);
        return hmacSha256.ComputeHash(data);
    }

    /// <summary>
    /// Derives a cryptographic key from a password using PBKDF2 with recommended values.
    /// </summary>
    /// <param name="password">The password to use for key derivation.</param>
    /// <param name="salt">The salt used to make the key derivation process more secure.</param>
    /// <returns>The derived key as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="password" /> or <paramref name="salt" /> is null.</exception>
    public static byte[] DeriveKeyFromPassword(byte[] password, byte[] salt)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        if (salt == null)
        {
            throw new ArgumentNullException(nameof(salt));
        }

        if (salt.Length < 16)
        {
            throw new ArgumentException(Resources.ArgumentException_SaltSizeMustBeAtLeast16Bytes, nameof(salt));
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }

    /// <summary>
    /// Generates a cryptographically secure key for HMAC-SHA256.
    /// </summary>
    /// <returns>A cryptographically secure key as a byte array, which is 32 bytes long.</returns>
    public static byte[] GenerateHmac256Key()
    {
        return GenerateRandomBytes(32);
    }

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
    /// Generates a cryptographically secure salt.
    /// </summary>
    /// <param name="saltSize">The size of the salt in bytes. Default is 16 bytes.</param>
    /// <returns>A cryptographically secure salt as a byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="saltSize" /> is less than or equal to zero.</exception>
    public static byte[] GenerateSalt(int saltSize = 16)
    {
        if (saltSize <= 0)
        {
            throw new ArgumentException(Resources.ArgumentException_SaltSizeMustBeGreaterThanZero, nameof(saltSize));
        }

        return GenerateRandomBytes(saltSize);
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