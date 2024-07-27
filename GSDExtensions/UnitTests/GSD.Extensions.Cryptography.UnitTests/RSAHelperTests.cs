// <copyright file="RSAHelperTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography.UnitTests;

using System.Text;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="RSAHelper" /> class.
/// </summary>
public class RSAHelperTests
{
    /// <summary>
    /// Tests the <see cref="RSAHelper.Encrypt" /> and <see cref="RSAHelper.Decrypt" /> methods to ensure that
    /// encryption and decryption work correctly.
    /// </summary>
    [Fact]
    public void EncryptAndDecryptReturnsOriginalData()
    {
        var (publicKey, privateKey) = RSAHelper.GenerateKeyPair();
        var data = Encoding.UTF8.GetBytes("Test data");

        var encryptedData = RSAHelper.Encrypt(data, publicKey);
        var decryptedData = RSAHelper.Decrypt(encryptedData, privateKey);

        Assert.Equal(data, decryptedData);
    }

    /// <summary>
    /// Tests the <see cref="RSAHelper.ExtractPublicKey" /> method to ensure it correctly extracts the public key
    /// from a private key.
    /// </summary>
    [Fact]
    public void ExtractPublicKeyReturnsPublicKey()
    {
        var (publicKey, privateKey) = RSAHelper.GenerateKeyPair();
        var extractedPublicKey = RSAHelper.ExtractPublicKey(privateKey);

        Assert.Equal(publicKey, extractedPublicKey);
    }

    /// <summary>
    /// Tests the <see cref="RSAHelper.GenerateKeyPair" /> method to ensure it returns valid keys.
    /// </summary>
    [Fact]
    public void GenerateKeyPairReturnsKeys()
    {
        var (publicKey, privateKey) = RSAHelper.GenerateKeyPair();

        Assert.NotNull(publicKey);
        Assert.NotNull(privateKey);
        Assert.NotEmpty(publicKey);
        Assert.NotEmpty(privateKey);
    }

    /// <summary>
    /// Tests the <see cref="RSAHelper.GeneratePrivateKey" /> method to ensure it returns a valid private key.
    /// </summary>
    [Fact]
    public void GeneratePrivateKeyReturnsKey()
    {
        var privateKey = RSAHelper.GeneratePrivateKey();

        Assert.NotNull(privateKey);
        Assert.NotEmpty(privateKey);
    }

    /// <summary>
    /// Tests the <see cref="RSAHelper.Sign" /> and <see cref="RSAHelper.Verify" /> methods to ensure that data can
    /// be signed and verified correctly.
    /// </summary>
    [Fact]
    public void SignAndVerifyReturnsTrue()
    {
        var (publicKey, privateKey) = RSAHelper.GenerateKeyPair();
        var data = Encoding.UTF8.GetBytes("Test data");

        var signature = RSAHelper.Sign(data, privateKey);
        var isValid = RSAHelper.Verify(data, signature, publicKey);

        Assert.True(isValid);
    }

    /// <summary>
    /// Tests the <see cref="RSAHelper.Verify" /> method to ensure that it returns false for an invalid signature.
    /// </summary>
    [Fact]
    public void VerifyInvalidSignatureReturnsFalse()
    {
        var (publicKey, _) = RSAHelper.GenerateKeyPair();
        var data = Encoding.UTF8.GetBytes("Test data");
        var invalidSignature = new byte[] { 0x00, 0x01, 0x02 };

        var isValid = RSAHelper.Verify(data, invalidSignature, publicKey);

        Assert.False(isValid);
    }
}