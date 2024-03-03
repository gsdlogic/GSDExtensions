// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoUtility.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Cryptography;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

/// <summary>
/// Provides methods for common cryptographic services.
/// </summary>
public static class CryptoUtility
{
    /// <summary>
    /// Decrypts data using the AES-256 algorithm.
    /// </summary>
    /// <param name="key">The key for the AES-256 algorithm.</param>
    /// <param name="value">The data to be decrypted prefixed with the initialization vector for the AES-256 algorithm.</param>
    /// <returns>The decrypted data.</returns>
    public static byte[] AES256Decrypt(byte[] key, byte[] value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var iv = new byte[16];
        var encryptedLength = value.Length - 16;
        var encrypted = new byte[encryptedLength];

        Buffer.BlockCopy(value, 0, iv, 0, 16);
        Buffer.BlockCopy(value, 16, encrypted, 0, encryptedLength);

        using var aes = new AesCryptoServiceProvider
        {
            Key = key,
            IV = iv,
        };

        using var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

        return decrypted;
    }

    /// <summary>
    /// Encrypts data using the AES-256 algorithm.
    /// </summary>
    /// <param name="key">The key for the AES-256 algorithm.</param>
    /// <param name="value">The data to be encrypted.</param>
    /// <returns>The encrypted data prefixed with the initialization vector for the AES-256 algorithm.</returns>
    public static byte[] AES256Encrypt(byte[] key, byte[] value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        using var aes = new AesCryptoServiceProvider
        {
            Key = key,
        };

        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(value, 0, value.Length);

        var result = new byte[aes.IV.Length + encrypted.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

        return result;
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong random sequence of values for use as a symmetric key in AES-256 encryption.
    /// </summary>
    /// <returns>The array of bytes with a cryptographically strong random sequence of values.</returns>
    public static byte[] GetAES256Key()
    {
        return GetRandomBytes(32);
    }

    /// <summary>
    /// Creates a PBKDF2 derived key from password bytes for use as a symmetric key in AES-256 encryption.
    /// </summary>
    /// <param name="password">The password to use to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key (recommend 16 bytes or greater).</param>
    /// <returns>A byte array containing the created PBKDF2 derived key.</returns>
    public static byte[] GetAES256KeyFromPassword(byte[] password, byte[] salt)
    {
        return GetPasswordHash(password, salt);
    }

    /// <summary>
    /// Creates a PBKDF2 derived key from password bytes.
    /// </summary>
    /// <param name="password">The password to use to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key (recommend 16 bytes or greater).</param>
    /// <param name="outputLength">The size of the key to derive (recommend 32 bytes for AES-256 encryption).</param>
    /// <returns>A byte array containing the created PBKDF2 derived key.</returns>
    public static byte[] GetPasswordHash(byte[] password, byte[] salt, int outputLength = 32)
    {
        return GetRfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256, outputLength);
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong random sequence of values for use as a password salt.
    /// </summary>
    /// <returns>The array of bytes with a cryptographically strong random sequence of values.</returns>
    public static byte[] GetPasswordSalt()
    {
        return GetRandomBytes(16);
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong random sequence of values.
    /// </summary>
    /// <param name="outputLength">The number of bytes to return (recommend 32 for AES-256 encryption and 16 for a password salt).</param>
    /// <returns>The array of bytes with a cryptographically strong random sequence of values.</returns>
    public static byte[] GetRandomBytes(int outputLength)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[outputLength];
        rng.GetBytes(bytes);
        return bytes;
    }

    /// <summary>
    /// Creates a <see cref="Guid" /> using a cryptographically strong random sequence of values.
    /// </summary>
    /// <returns>A <see cref="Guid" /> using a cryptographically strong random sequence of values.</returns>
    public static Guid GetRandomGuid()
    {
        var bytes = GetRandomBytes(16);
        bytes[8] = (byte)((bytes[8] & 0xBF) | 0x80);
        bytes[7] = (byte)((bytes[7] & 0x4F) | 0x40);
        return new Guid(bytes);
    }

    /// <summary>
    /// Creates an RSA private-key in the PKCS#1 format.
    /// </summary>
    /// <param name="keySize">The size of the key to use in bits.</param>
    /// <returns>An RSA private-key in the PKCS#1 format.</returns>
    public static byte[] GetRSAPrivateKey(int keySize = 2048)
    {
        using var rsa = new RSACryptoServiceProvider(keySize);
        return rsa.ExportRSAPrivateKey();
    }

    /// <summary>
    /// Exports the public-key portion of the RSA private-key in the PKCS#1 format.
    /// </summary>
    /// <param name="privateKey">The bytes of a PKCS#1 structure.</param>
    /// <returns>The public-key portion of the RSA private-key in the PKCS#1 format.</returns>
    public static byte[] GetRSAPublicKey(byte[] privateKey)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.ExportRSAPublicKey();
    }

    /// <summary>
    /// Decrypts data with a RSA private-key.
    /// </summary>
    /// <param name="privateKey">The RSA private-key in the PKCS#1 format.</param>
    /// <param name="value">The data to be decrypted.</param>
    /// <returns>The decrypted data.</returns>
    public static byte[] RSADecrypt(byte[] privateKey, byte[] value)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.Decrypt(value, false);
    }

    /// <summary>
    /// Encrypts data with a RSA public-key.
    /// </summary>
    /// <param name="publicKey">The RSA public-key in the PKCS#1 format.</param>
    /// <param name="value">The data to be encrypted.</param>
    /// <returns>The encrypted data.</returns>
    public static byte[] RSAEncrypt(byte[] publicKey, byte[] value)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.Encrypt(value, false);
    }

    /// <summary>
    /// Determines whether two byte arrays are equal in length-constant time.
    /// </summary>
    /// <param name="a">The first byte array to compare.</param>
    /// <param name="b">The second byte array to compare.</param>
    /// <returns><see langword="true" /> if the two byte arrays are of equal length and their corresponding elements are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>
    /// This comparison method is used so that password hashes cannot be extracted from on-line systems using a timing attack and then attacked off-line.
    /// </remarks>
    public static bool SlowEquals(byte[] a, byte[] b)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        var diff = (uint)a.Length ^ (uint)b.Length;

        for (var i = 0; (i < a.Length) && (i < b.Length); i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }

        return diff == 0;
    }

    /// <summary>
    /// Creates a PBKDF2 derived key from password bytes.
    /// </summary>
    /// <param name="password">The password to use to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key (recommend 16 bytes or greater).</param>
    /// <param name="iterations">The number of iterations for the operation (recommend 100,000 or greater).</param>
    /// <param name="hashAlgorithm">The hash algorithm to use to derive the key (recommend SHA-256).</param>
    /// <param name="outputLength">The size of the key to derive (recommend 32 bytes for AES-256 encryption).</param>
    /// <returns>A byte array containing the created PBKDF2 derived key.</returns>
    [SuppressMessage("Security", "CA5379:Ensure Key Derivation Function algorithm is sufficiently strong", Justification = "Callers of this method provide recommended values.")]
    private static byte[] GetRfc2898DeriveBytes(byte[] password, byte[] salt, int iterations, HashAlgorithmName hashAlgorithm, int outputLength)
    {
        using var derivedBytes = new Rfc2898DeriveBytes(password, salt, iterations, hashAlgorithm);
        return derivedBytes.GetBytes(outputLength);
    }
}