// <copyright file="HashHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using System.Security.Cryptography;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="HashHelper" /> class.
/// </summary>
public class HashHelperTests
{
    /// <summary>
    /// Verifies that the HMAC-SHA256 computation produces the expected hash output.
    /// </summary>
    [Fact]
    public void ComputeHmacSha256ShouldReturnExpectedHash()
    {
        // Arrange
        var key = new byte[]
        {
            0x0A, 0x1B, 0x2C, 0x3D, 0x4E, 0x5F, 0x6A, 0x7B, // Example key
            0x8C, 0x9D, 0xAE, 0xBF, 0xCA, 0xDB, 0xEC, 0xFD,
        };

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        using var hmacSha256 = new HMACSHA256(key);
        var expectedHash = hmacSha256.ComputeHash(data);

        // Act
        var actualHash = HashHelper.ComputeHmacSha256(key, data);

        // Assert
        Assert.Equal(expectedHash, actualHash);
    }

    /// <summary>
    /// Verifies that the SHA256 hash computation produces the expected hash output.
    /// </summary>
    [Fact]
    public void ComputeSha256HashShouldReturnExpectedHash()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var expectedHash = SHA256.HashData(data);

        // Act
        var actualHash = HashHelper.ComputeSha256Hash(data);

        // Assert
        Assert.Equal(expectedHash, actualHash);
    }

    /// <summary>
    /// Verifies that the SHA512 hash computation produces the expected hash output.
    /// </summary>
    [Fact]
    public void ComputeSha512HashShouldReturnExpectedHash()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var expectedHash = SHA512.HashData(data);

        // Act
        var actualHash = HashHelper.ComputeSha512Hash(data);

        // Assert
        Assert.Equal(expectedHash, actualHash);
    }

    /// <summary>
    /// Verifies that an exception is thrown if the salt size is less than the minimum recommended size.
    /// </summary>
    [Fact]
    public void DeriveKeyFromPasswordWithInvalidSaltShouldThrowArgumentException()
    {
        // Arrange
        var password = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var salt = new byte[15]; // Salt size less than recommended

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => HashHelper.DeriveKeyFromPassword(password, salt));
        Assert.Equal("Salt size must be at least 16 bytes. (Parameter 'salt')", exception.Message);
    }

    /// <summary>
    /// Verifies that the generated HMAC-SHA256 key is of the expected size.
    /// </summary>
    [Fact]
    public void GenerateHmac256KeyShouldReturnKeyOfExpectedSize()
    {
        // Act
        var key = HashHelper.GenerateHmac256Key();

        // Assert
        Assert.Equal(32, key.Length); // HMAC-SHA256 key size is 32 bytes
    }

    /// <summary>
    /// Verifies that a custom salt size is respected.
    /// </summary>
    [Fact]
    public void GenerateSaltWithCustomSizeShouldReturnSaltOfExpectedSize()
    {
        // Arrange
        const int SaltSize = 32;

        // Act
        var salt = HashHelper.GenerateSalt(SaltSize);

        // Assert
        Assert.Equal(SaltSize, salt.Length); // Custom salt size
    }

    /// <summary>
    /// Verifies that the default salt size is 16 bytes.
    /// </summary>
    [Fact]
    public void GenerateSaltWithDefaultSizeShouldReturnSaltOfExpectedSize()
    {
        // Act
        var salt = HashHelper.GenerateSalt();

        // Assert
        Assert.Equal(16, salt.Length); // Default salt size is 16 bytes
    }

    /// <summary>
    /// Verifies that an exception is thrown when an invalid salt size is provided.
    /// </summary>
    [Fact]
    public void GenerateSaltWithInvalidSizeShouldThrowArgumentException()
    {
        // Arrange
        const int InvalidSaltSize = 0;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => HashHelper.GenerateSalt(InvalidSaltSize));
        Assert.Equal("Salt size must be greater than zero. (Parameter 'saltSize')", exception.Message);
    }
}