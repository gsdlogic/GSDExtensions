// <copyright file="ECDsaHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using System.Text;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="ECDsaHelper" /> class.
/// </summary>
/// ReSharper disable once InconsistentNaming
public class ECDsaHelperTests
{
    /// <summary>
    /// Tests that the public key can be extracted from a private key.
    /// </summary>
    [Fact]
    public void ExtractPublicKeyFromPrivateKeyShouldReturnPublicKey()
    {
        // Arrange
        var (publicKey, privateKey) = ECDsaHelper.GenerateKeyPair();

        // Act
        var extractedPublicKey = ECDsaHelper.ExtractPublicKey(privateKey);

        // Assert
        Assert.NotNull(extractedPublicKey);
        Assert.Equal(publicKey, extractedPublicKey);
    }

    /// <summary>
    /// Tests that key pair generation returns valid keys.
    /// </summary>
    [Fact]
    public void GenerateKeyPairShouldReturnKeys()
    {
        // Act
        var (publicKey, privateKey) = ECDsaHelper.GenerateKeyPair();

        // Assert
        Assert.NotNull(publicKey);
        Assert.NotNull(privateKey);
        Assert.NotEmpty(publicKey);
        Assert.NotEmpty(privateKey);
    }

    /// <summary>
    /// Tests that generating a private key alone returns a valid key.
    /// </summary>
    [Fact]
    public void GeneratePrivateKeyShouldReturnPrivateKey()
    {
        // Act
        var privateKey = ECDsaHelper.GeneratePrivateKey();

        // Assert
        Assert.NotNull(privateKey);
        Assert.NotEmpty(privateKey);
    }

    /// <summary>
    /// Tests that signing data with a valid private key returns a signature.
    /// </summary>
    [Fact]
    public void SignValidDataAndPrivateKeyShouldReturnSignature()
    {
        // Arrange
        var (_, privateKey) = ECDsaHelper.GenerateKeyPair();
        var data = Encoding.UTF8.GetBytes("test data");

        // Act
        var signature = ECDsaHelper.Sign(data, privateKey);

        // Assert
        Assert.NotNull(signature);
        Assert.NotEmpty(signature);
    }

    /// <summary>
    /// Tests that a valid signature can be verified with the correct public key.
    /// </summary>
    [Fact]
    public void VerifyValidSignatureShouldReturnTrue()
    {
        // Arrange
        var (publicKey, privateKey) = ECDsaHelper.GenerateKeyPair();
        var data = Encoding.UTF8.GetBytes("test data");
        var signature = ECDsaHelper.Sign(data, privateKey);

        // Act
        var isValid = ECDsaHelper.Verify(data, signature, publicKey);

        // Assert
        Assert.True(isValid);
    }
}