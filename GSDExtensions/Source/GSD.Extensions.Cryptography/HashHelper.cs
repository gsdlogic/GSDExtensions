// <copyright file="HashHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;

/// <summary>
/// Provides static methods for performing various cryptographic operations.
/// </summary>
public static class HashHelper
{
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
}