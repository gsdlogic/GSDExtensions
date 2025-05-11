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
}