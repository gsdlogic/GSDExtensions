// <copyright file="HashHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;
using GSD.Extensions.Cryptography.Properties;

/// <summary>
/// Provides static methods for performing various cryptographic operations.
/// </summary>
public static class HashHelper
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
    /// Computes the SHA-256 hash of the specified data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>The SHA-256 hash as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is null.</exception>
    public static byte[] ComputeSha256Hash(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(data);
    }

    /// <summary>
    /// Computes the SHA-512 hash of the specified data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>The SHA-512 hash as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is null.</exception>
    public static byte[] ComputeSha512Hash(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using var sha512 = SHA512.Create();
        return sha512.ComputeHash(data);
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
        return CryptoHelper.GenerateRandomBytes(32);
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

        return CryptoHelper.GenerateRandomBytes(saltSize);
    }
}