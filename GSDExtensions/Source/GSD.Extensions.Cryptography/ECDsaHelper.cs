// <copyright file="ECDsaHelper.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Cryptography;

using System;
using System.Security.Cryptography;

/// <summary>
/// Provides Elliptic Curve Cryptography (ECC) utilities for key generation, signing, verifying, and extracting public keys.
/// </summary>
/// ReSharper disable PossibleNullReferenceException
/// ReSharper disable once InconsistentNaming
public static class ECDsaHelper
{
    /// <summary>
    /// Extracts the public key from the given ECC private key.
    /// </summary>
    /// <param name="privateKey">The ECC private key as a byte array.</param>
    /// <returns>The public key derived from the private key as a byte array.</returns>
    public static byte[] ExtractPublicKey(byte[] privateKey)
    {
        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportECPrivateKey(privateKey, out _);
        return ecdsa.ExportSubjectPublicKeyInfo();
    }

    /// <summary>
    /// Generates a new ECC key pair.
    /// </summary>
    /// <returns>A tuple containing the public key and private key as byte arrays.</returns>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateKeyPair()
    {
        using var ecdsa = ECDsa.Create();
        return (ecdsa.ExportSubjectPublicKeyInfo(), ecdsa.ExportECPrivateKey());
    }

    /// <summary>
    /// Generates a new ECC private key.
    /// </summary>
    /// <returns>The ECC private key as a byte array.</returns>
    public static byte[] GeneratePrivateKey()
    {
        using var ecdsa = ECDsa.Create();
        return ecdsa.ExportECPrivateKey();
    }

    /// <summary>
    /// Signs the specified data using ECC private key.
    /// </summary>
    /// <param name="data">The data to sign as a byte array.</param>
    /// <param name="privateKey">The ECC private key as a byte array.</param>
    /// <returns>The signature as a byte array.</returns>
    public static byte[] Sign(byte[] data, byte[] privateKey)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (privateKey == null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportECPrivateKey(privateKey, out _);
        return ecdsa.SignData(data, HashAlgorithmName.SHA256);
    }

    /// <summary>
    /// Verifies the signature of the specified data using ECC public key.
    /// </summary>
    /// <param name="data">The data whose signature is to be verified as a byte array.</param>
    /// <param name="signature">The signature to verify as a byte array.</param>
    /// <param name="publicKey">The ECC public key as a byte array.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public static bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (signature == null)
        {
            throw new ArgumentNullException(nameof(signature));
        }

        if (publicKey == null)
        {
            throw new ArgumentNullException(nameof(publicKey));
        }

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportSubjectPublicKeyInfo(publicKey, out _);
        return ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
    }
}