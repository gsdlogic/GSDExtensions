// <copyright file="CryptoHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using System.Security.Cryptography;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="CryptoHelper" /> class.
/// </summary>
public class CryptoHelperTests
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
        var actualHash = CryptoHelper.ComputeHmacSha256(key, data);

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
        var exception = Assert.Throws<ArgumentException>(() => CryptoHelper.DeriveKeyFromPassword(password, salt));
        Assert.Equal("Salt size must be at least 16 bytes. (Parameter 'salt')", exception.Message);
    }

    /// <summary>
    /// Verifies that the generated HMAC-SHA256 key is of the expected size.
    /// </summary>
    [Fact]
    public void GenerateHmac256KeyShouldReturnKeyOfExpectedSize()
    {
        // Act
        var key = CryptoHelper.GenerateHmac256Key();

        // Assert
        Assert.Equal(32, key.Length); // HMAC-SHA256 key size is 32 bytes
    }

    /// <summary>
    /// Verifies that GenerateRandomBytes does not return a null value.
    /// </summary>
    [Fact]
    public void GenerateRandomBytesShouldNotReturnNull()
    {
        // Arrange
        const int Size = 32; // Desired size of the random byte array

        // Act
        var result = CryptoHelper.GenerateRandomBytes(Size);

        // Assert
        Assert.NotNull(result); // Ensure the result is not null
    }

    /// <summary>
    /// Verifies that GenerateRandomBytes returns a byte array of the requested size.
    /// </summary>
    [Fact]
    public void GenerateRandomBytesShouldReturnByteArrayOfExpectedSize()
    {
        // Arrange
        const int Size = 32; // Desired size of the random byte array

        // Act
        var result = CryptoHelper.GenerateRandomBytes(Size);

        // Assert
        Assert.Equal(Size, result.Length); // Check if the length matches the requested size
    }

    /// <summary>
    /// Verifies that GenerateRandomBytes handles zero size correctly by throwing an exception.
    /// </summary>
    [Fact]
    public void GenerateRandomBytesWithZeroSizeShouldThrowArgumentException()
    {
        // Arrange
        const int Size = 0; // Invalid size

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => CryptoHelper.GenerateRandomBytes(Size));
        Assert.Equal("Size must be greater than zero. (Parameter 'size')", exception.Message);
    }

    /// <summary>
    /// Verifies that GenerateRandomGuid returns a valid GUID.
    /// </summary>
    [Fact]
    public void GenerateRandomGuidShouldReturnValidGuid()
    {
        // Act
        var guid = CryptoHelper.GenerateRandomGuid();

        // Assert
        Assert.NotEqual(Guid.Empty, guid);
        Assert.Equal(16, guid.ToByteArray().Length);

        var bytes = guid.ToByteArray();

        // Validate version and variant bits
        Assert.Equal(0x80, (byte)(bytes[8] & 0xC0)); // Variant RFC 4122
        Assert.Equal(0x40, (byte)(bytes[7] & 0xF0)); // Version 4
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
        var salt = CryptoHelper.GenerateSalt(SaltSize);

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
        var salt = CryptoHelper.GenerateSalt();

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
        var exception = Assert.Throws<ArgumentException>(() => CryptoHelper.GenerateSalt(InvalidSaltSize));
        Assert.Equal("Salt size must be greater than zero. (Parameter 'saltSize')", exception.Message);
    }

    /// <summary>
    /// Verifies that SlowEquals returns false for byte arrays of different lengths.
    /// </summary>
    [Fact]
    public void SlowEqualsReturnsFalseForDifferentLengthByteArrays()
    {
        // Arrange
        var array1 = new byte[] { 0x01, 0x02, 0x03 };
        var array2 = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        // Act
        var result = CryptoHelper.SlowEquals(array1, array2);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that SlowEquals handles null byte arrays gracefully.
    /// </summary>
    [Fact]
    public void SlowEqualsReturnsFalseForNullByteArrays()
    {
        // Act
        var result = CryptoHelper.SlowEquals(null, null);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that SlowEquals correctly identifies unequal byte arrays.
    /// </summary>
    [Fact]
    public void SlowEqualsReturnsFalseForUnequalByteArrays()
    {
        // Arrange
        var array1 = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var array2 = new byte[] { 0x01, 0x02, 0x03, 0x05 };

        // Act
        var result = CryptoHelper.SlowEquals(array1, array2);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that SlowEquals correctly identifies equal byte arrays.
    /// </summary>
    [Fact]
    public void SlowEqualsReturnsTrueForEqualByteArrays()
    {
        // Arrange
        var array1 = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var array2 = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        // Act
        var result = CryptoHelper.SlowEquals(array1, array2);

        // Assert
        Assert.True(result);
    }
}