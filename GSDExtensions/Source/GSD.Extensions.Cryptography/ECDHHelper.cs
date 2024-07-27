// <copyright file="ECDHHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;

/// <summary>
/// Provides Elliptic Curve Diffie-Hellman (ECDH) utilities for key generation, key exchange, and key extraction.
/// </summary>
/// ReSharper disable once InconsistentNaming
public static class ECDHHelper
{
    /// <summary>
    /// Derives a shared secret from the private key and the public key of another party.
    /// </summary>
    /// <param name="privateKey">The private key as a byte array.</param>
    /// <param name="publicKey">The public key of the other party as a byte array.</param>
    /// <returns>The derived shared secret as a byte array.</returns>
    public static byte[] DeriveSharedSecret(byte[] privateKey, byte[] publicKey)
    {
        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        if (publicKey == null)
        {
            throw new ArgumentNullException(nameof(publicKey));
        }

        using var dh1 = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        dh1.ImportECPrivateKey(privateKey, out _);

        using var dh2 = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        dh2.ImportSubjectPublicKeyInfo(publicKey, out _);

        return dh1.DeriveKeyMaterial(dh2.PublicKey);
    }

    /// <summary>
    /// Extracts the public key from the given ECDH private key.
    /// </summary>
    /// <param name="privateKey">The ECDH private key as a byte array.</param>
    /// <returns>The public key derived from the private key as a byte array.</returns>
    public static byte[] ExtractPublicKey(byte[] privateKey)
    {
        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        using var dh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        dh.ImportECPrivateKey(privateKey, out _);
        return dh.ExportSubjectPublicKeyInfo();
    }

    /// <summary>
    /// Generates a new ECDH key pair.
    /// </summary>
    /// <returns>A tuple containing the public key and private key as byte arrays.</returns>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateKeyPair()
    {
        using var dh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        var publicKey = dh.ExportSubjectPublicKeyInfo(); // Export public key in SubjectPublicKeyInfo format
        var privateKey = dh.ExportECPrivateKey(); // Export private key
        return (publicKey, privateKey);
    }

    /// <summary>
    /// Generates a new ECDH private key.
    /// </summary>
    /// <returns>The private key as a byte array.</returns>
    public static byte[] GeneratePrivateKey()
    {
        using var dh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        return dh.ExportECPrivateKey();
    }
}