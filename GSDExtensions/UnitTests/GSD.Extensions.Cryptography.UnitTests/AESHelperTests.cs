// <copyright file="AESHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using System.Security.Cryptography;
using System.Text;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="AESHelper" /> class.
/// </summary>
public class AESHelperTests
{
    /// <summary>
    /// Tests that the <see cref="AESHelper.Decrypt(byte[], byte[], byte[])" /> method returns the original plain text.
    /// </summary>
    [Fact]
    public void DecryptReturnsOriginalPlainText()
    {
        // Arrange
        var key = new byte[16]; // 128-bit key for AES
        var plainText = Encoding.UTF8.GetBytes("Test message");
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        var (cipherText, iv) = AESHelper.Encrypt(plainText, key);

        // Act
        var decrypted = AESHelper.Decrypt(cipherText, key, iv);

        // Assert
        Assert.NotNull(decrypted);
        Assert.Equal(plainText, decrypted);
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.Decrypt(byte[], byte[], byte[])" /> method throws an <see cref="ArgumentNullException" /> when the cipher text is null.
    /// </summary>
    [Fact]
    public void DecryptThrowsArgumentNullExceptionWhenCipherTextIsNull()
    {
        // Arrange
        var key = new byte[16]; // 128-bit key for AES
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AESHelper.Decrypt(null, key, new byte[16]));
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.Decrypt(byte[], byte[], byte[])" /> method throws an <see cref="ArgumentNullException" /> when the initialization vector (IV) is null.
    /// </summary>
    [Fact]
    public void DecryptThrowsArgumentNullExceptionWhenIVIsNull()
    {
        // Arrange
        var key = new byte[16]; // 128-bit key for AES
        var plainText = Encoding.UTF8.GetBytes("Test message");
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        var cipherText = AESHelper.Encrypt(plainText, key).CipherText;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AESHelper.Decrypt(cipherText, key, null));
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.Decrypt(byte[], byte[], byte[])" /> method throws an <see cref="ArgumentNullException" /> when the key is null.
    /// </summary>
    [Fact]
    public void DecryptThrowsArgumentNullExceptionWhenKeyIsNull()
    {
        // Arrange
        var plainText = Encoding.UTF8.GetBytes("Test message");
        var iv = new byte[16]; // Dummy IV

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AESHelper.Decrypt(plainText, null, iv));
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.Encrypt(byte[], byte[])" /> method returns non-null and non-empty cipher text, and a valid initialization vector (IV).
    /// </summary>
    [Fact]
    public void EncryptReturnsCipherTextAndIV()
    {
        // Arrange
        var key = new byte[16]; // 128-bit key for AES
        var plainText = Encoding.UTF8.GetBytes("Test message");
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        // Act
        var (cipherText, iv) = AESHelper.Encrypt(plainText, key);

        // Assert
        Assert.NotNull(cipherText);
        Assert.NotEmpty(cipherText);
        Assert.NotNull(iv);
        Assert.Equal(16, iv.Length); // AES IV length should be 16 bytes
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.Encrypt(byte[], byte[])" /> method throws an <see cref="ArgumentNullException" /> when the key is null.
    /// </summary>
    [Fact]
    public void EncryptThrowsArgumentNullExceptionWhenKeyIsNull()
    {
        // Arrange
        var plainText = Encoding.UTF8.GetBytes("Test message");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AESHelper.Encrypt(plainText, null));
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.Encrypt(byte[], byte[])" /> method throws an <see cref="ArgumentNullException" /> when the plain text is null.
    /// </summary>
    [Fact]
    public void EncryptThrowsArgumentNullExceptionWhenPlainTextIsNull()
    {
        // Arrange
        var key = new byte[16]; // 128-bit key for AES
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AESHelper.Encrypt(null, key));
    }

    /// <summary>
    /// Tests that the <see cref="AESHelper.GenerateKey" /> method returns a non-null key of the expected length.
    /// </summary>
    [Fact]
    public void GenerateKeyReturnsValidKey()
    {
        // Act
        var key = AESHelper.GenerateKey();

        // Assert
        Assert.NotNull(key);
        Assert.Equal(32, key.Length); // AES key length should be 256 bits (32 bytes)
    }
}