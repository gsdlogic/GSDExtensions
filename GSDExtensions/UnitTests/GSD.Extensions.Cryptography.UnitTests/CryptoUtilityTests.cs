// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoUtilityTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Cryptography.UnitTests;

using System.Security.Cryptography;
using System.Text;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="CryptoUtility" /> class.
/// </summary>
public class CryptoUtilityTests
{
    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.AES256Encrypt(byte[], byte[])" /> and <see cref="CryptoUtility.AES256Decrypt(byte[], byte[])" /> methods.
    /// </summary>
    [Fact]
    public void AES256EncryptionTest()
    {
        var key = CryptoUtility.GetAES256Key();
        var data = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1A };
        var encrypted = CryptoUtility.AES256Encrypt(key, data);
        var decrypted = CryptoUtility.AES256Decrypt(key, encrypted);
        Assert.True(decrypted.SequenceEqual(data));
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetAES256KeyFromPassword(byte[], byte[])" /> method.
    /// </summary>
    [Fact]
    public void GetAES256KeyFromPasswordTest()
    {
        var password = Encoding.UTF8.GetBytes("password");
        var salt = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1A };
        var value1 = CryptoUtility.GetAES256KeyFromPassword(password, salt);
        var value2 = CryptoUtility.GetAES256KeyFromPassword(password, salt);
        Assert.Equal(32, value1.Length);
        Assert.True(value2.SequenceEqual(value1));
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetAES256Key()" /> method.
    /// </summary>
    [Fact]
    public void GetAES256KeyTests()
    {
        var bytes = CryptoUtility.GetAES256Key();
        Assert.Equal(32, bytes.Length);
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetPasswordHash(byte[], byte[], int)" /> method.
    /// </summary>
    [Fact]
    public void GetPasswordHashTest()
    {
        var password = Encoding.UTF8.GetBytes("password");
        var salt = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1A };
        var value1 = CryptoUtility.GetPasswordHash(password, salt);
        var value2 = CryptoUtility.GetPasswordHash(password, salt);
        Assert.Equal(32, value1.Length);
        Assert.True(value2.SequenceEqual(value1));
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetPasswordSalt()" /> method.
    /// </summary>
    [Fact]
    public void GetPasswordSaltTest()
    {
        var bytes = CryptoUtility.GetPasswordSalt();
        Assert.Equal(16, bytes.Length);
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetRandomBytes(int)" /> method.
    /// </summary>
    [Fact]
    public void GetRandomBytesTest()
    {
        var bytes = CryptoUtility.GetRandomBytes(32);
        Assert.Equal(32, bytes.Length);
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetRandomGuid" /> method.
    /// </summary>
    [Fact]
    public void GetRandomGuidTest()
    {
        var result = CryptoUtility.GetRandomGuid();
        var bytes = result.ToByteArray();
        Assert.Equal(0x80, (byte)(bytes[8] & 0xC0));
        Assert.Equal(0x40, (byte)(bytes[7] & 0xF0));
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetRSAPrivateKey(int)" /> method.
    /// </summary>
    [Fact]
    public void GetRSAPrivateKeyTest()
    {
        var privateKey = CryptoUtility.GetRSAPrivateKey();
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportRSAPrivateKey(privateKey, out var bytesRead);
        Assert.Equal(privateKey.Length, bytesRead);
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.GetRSAPublicKey(byte[])" /> method.
    /// </summary>
    [Fact]
    public void GetRSAPublicKeyTest()
    {
        var privateKey = CryptoUtility.GetRSAPrivateKey();
        var publicKey = CryptoUtility.GetRSAPublicKey(privateKey);
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportRSAPublicKey(publicKey, out var bytesRead);
        Assert.Equal(publicKey.Length, bytesRead);
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.RSAEncrypt(byte[], byte[])" /> and <see cref="CryptoUtility.RSAEncrypt(byte[], byte[])" /> methods.
    /// </summary>
    [Fact]
    public void RSAEncryptionTest()
    {
        var privateKey = CryptoUtility.GetRSAPrivateKey();
        var publicKey = CryptoUtility.GetRSAPublicKey(privateKey);
        var data = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1A };
        var encrypted = CryptoUtility.RSAEncrypt(publicKey, data);
        var decrypted = CryptoUtility.RSADecrypt(privateKey, encrypted);
        Assert.True(decrypted.SequenceEqual(data));
    }

    /// <summary>
    /// Provides a test for the <see cref="CryptoUtility.SlowEquals(byte[], byte[])" /> method.
    /// </summary>
    [Fact]
    public void SlowEqualsTest()
    {
        var a = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1A };
        var b = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1A };
        var c = new byte[] { 0x2A, 0x65, 0x6D, 0x9D, 0x36, 0x06, 0xC1, 0xE1, 0xE9, 0x47, 0x47, 0x7B, 0x7F, 0x11, 0x0D, 0x1B };

        Assert.True(CryptoUtility.SlowEquals(a, b));
        Assert.False(CryptoUtility.SlowEquals(a, c));
    }
}