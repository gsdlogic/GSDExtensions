// <copyright file="ECDHHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using Xunit;

/// <summary>
/// Unit tests for the <see cref="ECDHHelper" /> class.
/// </summary>
/// ReSharper disable once InconsistentNaming
public class ECDHHelperTests
{
    /// <summary>
    /// Tests that deriving a shared secret with a null private key throws an <see cref="ArgumentNullException" />.
    /// </summary>
    [Fact]
    public void DeriveSharedSecretWithNullPrivateKeyThrowsArgumentNullException()
    {
        // Arrange
        var (_, publicKey) = ECDHHelper.GenerateKeyPair();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ECDHHelper.DeriveSharedSecret(null, publicKey));
    }

    /// <summary>
    /// Tests that deriving a shared secret with a null public key throws an <see cref="ArgumentNullException" />.
    /// </summary>
    [Fact]
    public void DeriveSharedSecretWithNullPublicKeyThrowsArgumentNullException()
    {
        // Arrange
        var (privateKey, _) = ECDHHelper.GenerateKeyPair();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ECDHHelper.DeriveSharedSecret(privateKey, null));
    }

    /// <summary>
    /// Tests that deriving a shared secret with valid keys returns a non-null and non-empty shared secret and that it matches when the roles are reversed.
    /// </summary>
    [Fact]
    public void DeriveSharedSecretWithValidKeysReturnsSharedSecret()
    {
        // Arrange
        var (publicKey1, privateKey1) = ECDHHelper.GenerateKeyPair();
        var (publicKey2, privateKey2) = ECDHHelper.GenerateKeyPair();

        // Act
        var sharedSecret1 = ECDHHelper.DeriveSharedSecret(privateKey1, publicKey2);
        var sharedSecret2 = ECDHHelper.DeriveSharedSecret(privateKey2, publicKey1);

        // Assert
        Assert.NotNull(sharedSecret1);
        Assert.NotNull(sharedSecret2);
        Assert.NotEmpty(sharedSecret1);
        Assert.NotEmpty(sharedSecret2);
        Assert.Equal(sharedSecret1, sharedSecret2); // The shared secrets should match
    }

    /// <summary>
    /// Tests that extracting a public key with a null private key throws an <see cref="ArgumentNullException" />.
    /// </summary>
    [Fact]
    public void ExtractPublicKeyWithNullPrivateKeyThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ECDHHelper.ExtractPublicKey(null));
    }

    /// <summary>
    /// Tests that extracting a public key from a valid private key returns the expected public key.
    /// </summary>
    [Fact]
    public void ExtractPublicKeyWithValidPrivateKeyReturnsPublicKey()
    {
        // Arrange
        var (publicKey, privateKey) = ECDHHelper.GenerateKeyPair();

        // Act
        var extractedPublicKey = ECDHHelper.ExtractPublicKey(privateKey);

        // Assert
        Assert.NotNull(extractedPublicKey);
        Assert.NotEmpty(extractedPublicKey);
        Assert.Equal(publicKey, extractedPublicKey); // The extracted public key should match the original
    }

    /// <summary>
    /// Tests that generating a key pair returns non-null and non-empty public and private keys.
    /// </summary>
    [Fact]
    public void GenerateKeyPairReturnsNonNullValues()
    {
        // Act
        var (publicKey, privateKey) = ECDHHelper.GenerateKeyPair();

        // Assert
        Assert.NotNull(publicKey);
        Assert.NotNull(privateKey);
        Assert.NotEmpty(publicKey);
        Assert.NotEmpty(privateKey);
    }

    /// <summary>
    /// Tests that generating a private key returns a non-null and non-empty private key.
    /// </summary>
    [Fact]
    public void GeneratePrivateKeyReturnsNonNullValue()
    {
        // Act
        var privateKey = ECDHHelper.GeneratePrivateKey();

        // Assert
        Assert.NotNull(privateKey);
        Assert.NotEmpty(privateKey);
    }
}