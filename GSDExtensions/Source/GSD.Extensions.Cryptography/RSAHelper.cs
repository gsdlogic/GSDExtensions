// <copyright file="RSAHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;

/// <summary>
/// Provides RSA encryption, decryption, signing, and verification utilities.
/// </summary>
public static class RSAHelper
{
    /// <summary>
    /// Decrypts the specified cipher text using RSA private key.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <param name="privateKey">The RSA private key as a byte array.</param>
    /// <returns>The decrypted plain text as a byte array.</returns>
    public static byte[] Decrypt(byte[] cipherText, byte[] privateKey)
    {
        if (cipherText == null)
        {
            throw new ArgumentNullException(nameof(cipherText));
        }

        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.Decrypt(cipherText, RSAEncryptionPadding.OaepSHA256);
    }

    /// <summary>
    /// Encrypts the specified plain text using RSA public key.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <param name="publicKey">The RSA public key as a byte array.</param>
    /// <returns>The encrypted text as a byte array.</returns>
    public static byte[] Encrypt(byte[] plainText, byte[] publicKey)
    {
        if (plainText == null)
        {
            throw new ArgumentNullException(nameof(plainText));
        }

        if (publicKey == null)
        {
            throw new ArgumentNullException(nameof(publicKey));
        }

        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.Encrypt(plainText, RSAEncryptionPadding.OaepSHA256);
    }

    /// <summary>
    /// Extracts the RSA public key from the provided private key.
    /// </summary>
    /// <param name="privateKey">The RSA private key as a byte array.</param>
    /// <returns>The RSA public key as a byte array.</returns>
    public static byte[] ExtractPublicKey(byte[] privateKey)
    {
        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.ExportRSAPublicKey();
    }

    /// <summary>
    /// Generates a new RSA key pair.
    /// </summary>
    /// <returns>A tuple containing the public key and private key as byte arrays.</returns>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateKeyPair()
    {
        using var rsa = RSA.Create();
        return (rsa.ExportRSAPublicKey(), rsa.ExportRSAPrivateKey());
    }

    /// <summary>
    /// Generates a new RSA private key.
    /// </summary>
    /// <returns>The RSA private key as a byte array.</returns>
    public static byte[] GeneratePrivateKey()
    {
        using var rsa = RSA.Create();
        return rsa.ExportRSAPrivateKey();
    }

    /// <summary>
    /// Signs the specified data using RSA private key.
    /// </summary>
    /// <param name="data">The data to sign as a byte array.</param>
    /// <param name="privateKey">The RSA private key as a byte array.</param>
    /// <returns>The signature as a byte array.</returns>
    public static byte[] Sign(byte[] data, byte[] privateKey)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// Verifies the signature of the specified data using RSA public key.
    /// </summary>
    /// <param name="data">The data whose signature is to be verified as a byte array.</param>
    /// <param name="signature">The signature to verify as a byte array.</param>
    /// <param name="publicKey">The RSA public key as a byte array.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public static bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (signature == null)
        {
            throw new ArgumentNullException(nameof(signature));
        }

        if (publicKey == null)
        {
            throw new ArgumentNullException(nameof(publicKey));
        }

        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}