// <copyright file="AESHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;

/// <summary>
/// Provides helper methods for AES encryption and decryption.
/// </summary>
public static class AESHelper
{
    /// <summary>
    /// Decrypts the specified cipher text using AES encryption.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <param name="key">The decryption key.</param>
    /// <param name="iv">The initialization vector (IV) used for encryption.</param>
    /// <returns>The decrypted text.</returns>
    public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv)
    {
        if (cipherText == null)
        {
            throw new ArgumentNullException(nameof(cipherText));
        }

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (iv == null)
        {
            throw new ArgumentNullException(nameof(iv));
        }

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

        return decrypted;
    }

    /// <summary>
    /// Encrypts the specified plain text using AES encryption.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <param name="key">The encryption key.</param>
    /// <returns>The encrypted text and the initialization vector (IV) used for encryption.</returns>
    public static (byte[] CipherText, byte[] IV) Encrypt(byte[] plainText, byte[] key)
    {
        if (plainText == null)
        {
            throw new ArgumentNullException(nameof(plainText));
        }

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        using var aes = Aes.Create();
        aes.Key = key;

        using var encryptor = aes.CreateEncryptor();
        var cipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

        return (cipherText, aes.IV);
    }

    /// <summary>
    /// Generates a new random key for AES encryption.
    /// </summary>
    /// <returns>A byte array containing the AES key.</returns>
    public static byte[] GenerateKey()
    {
        using var aes = Aes.Create();
        return aes.Key;
    }
}