// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoUtility.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Cryptography;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Provides methods for common cryptographic services.
/// </summary>
public static class CryptoUtility
{
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
    /// Creates a PBKDF2 derived key from password bytes for use as a symmetric key in AES-256 encryption.
    /// </summary>
    /// <param name="password">The password to use to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key encoded with base-64 digits (recommend 16 bytes or greater).</param>
    /// <returns>A byte array containing the created PBKDF2 derived key encoded with base-64 digits.</returns>
    public static string GetBase64AES256FromPasswordKey(string password, string salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltBytes = Convert.FromBase64String(salt);
        var bytes = GetAES256KeyFromPassword(passwordBytes, saltBytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong random sequence of values for use as a symmetric key in AES-256 encryption.
    /// </summary>
    /// <returns>The array of bytes with a cryptographically strong random sequence of values encoded with base-64 digits.</returns>
    public static string GetBase64AES256Key()
    {
        var bytes = GetRandomBytes(32);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Creates a PBKDF2 derived key from password bytes.
    /// </summary>
    /// <param name="password">The password to use to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key encoded with base-64 digits (recommend 16 bytes or greater).</param>
    /// <param name="outputLength">The size of the key to derive (recommend 32 bytes for AES-256 encryption).</param>
    /// <returns>A byte array containing the created PBKDF2 derived key encoded with base-64 digits.</returns>
    public static string GetBase64PasswordHash(string password, string salt, int outputLength = 32)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltBytes = Convert.FromBase64String(salt);
        var bytes = GetPasswordHash(passwordBytes, saltBytes, outputLength);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong random sequence of values for use as a password salt.
    /// </summary>
    /// <returns>The array of bytes with a cryptographically strong random sequence of values encoded with base-64 digits.</returns>
    public static string GetBase64PasswordSalt()
    {
        var bytes = GetPasswordSalt();
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong random sequence of values.
    /// </summary>
    /// <param name="outputLength">The number of bytes to return (recommend 32 for AES-256 encryption and 16 for a password salt).</param>
    /// <returns>The array of bytes with a cryptographically strong random sequence of values encoded with base-64 digits.</returns>
    public static string GetBase64RandomBytes(int outputLength)
    {
        var bytes = GetRandomBytes(outputLength);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Creates an RSA private-key in the PKCS#1 format.
    /// </summary>
    /// <param name="keySize">The size of the key to use in bits.</param>
    /// <returns>An RSA private-key in the PKCS#1 format encoded with base-64 digits.</returns>
    public static string GetBase64RSAPrivateKey(int keySize = 2048)
    {
        var bytes = GetRSAPrivateKey(keySize);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Exports the public-key portion of the RSA private-key in the PKCS#1 format.
    /// </summary>
    /// <param name="privateKey">The bytes of a PKCS#1 structure encoded with base-64 digits.</param>
    /// <returns>The public-key portion of the RSA private-key in the PKCS#1 format encoded with base-64 digits.</returns>
    public static string GetBase64RSAPublicKey(string privateKey)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKey);
        var publicKeyBytes = GetRSAPublicKey(privateKeyBytes);
        return Convert.ToBase64String(publicKeyBytes);
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
    /// Decrypts data with a RSA private-key.
    /// </summary>
    /// <param name="privateKey">The RSA private-key in the PKCS#1 format encoded with base-64 digits.</param>
    /// <param name="value">The data to be decrypted.</param>
    /// <returns>The decrypted data.</returns>
    public static byte[] RSADecrypt(string privateKey, byte[] value)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKey);
        return RSADecrypt(privateKeyBytes, value);
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
    /// Encrypts data with a RSA public-key.
    /// </summary>
    /// <param name="publicKey">The RSA public-key in the PKCS#1 format encoded with base-64 digits.</param>
    /// <param name="value">The data to be encrypted.</param>
    /// <returns>The encrypted data.</returns>
    public static byte[] RSAEncrypt(string publicKey, byte[] value)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        return RSAEncrypt(publicKeyBytes, value);
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