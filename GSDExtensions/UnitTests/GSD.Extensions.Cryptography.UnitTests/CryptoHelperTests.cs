// <copyright file="CryptoHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="CryptoHelper" /> class.
/// </summary>
public class CryptoHelperTests
{
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