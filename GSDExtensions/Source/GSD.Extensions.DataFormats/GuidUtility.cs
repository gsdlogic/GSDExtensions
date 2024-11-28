// <copyright file="GuidUtility.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.DataFormats;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using GSD.Extensions.DataFormats.Properties;

/// <summary>
/// Provides methods for working with a Universally Unique Identifier (UUID) aka. Globally Unique Identifier (GUID).
/// </summary>
/// <remarks>See <c>https://www.ietf.org/rfc/rfc4122.txt</c>.</remarks>
public static class GuidUtility
{
    /// <summary>
    /// The namespace UUID for fully-qualified domain names (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid DnsNamespace = new("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    /// The namespace UUID for ISO OIDs (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid OidNamespace = new("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    /// The namespace UUID for URLs (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid UrlNamespace = new("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    /// The namespace UUID for X.500 DN (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid X500Namespace = new("6ba7b814-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    /// Returns a <see cref="Guid" /> from a 16-byte byte array in network byte order (big-endian).
    /// </summary>
    /// <param name="value">The 16-byte byte array in network byte order (big-endian).</param>
    /// <returns>The <see cref="Guid" /> that was converted.</returns>
    public static Guid FromNetworkByteArray(byte[] value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (value.Length != 16)
        {
            throw new ArgumentException(Resources.ArgumentException_ArrayMustHaveLengthOf16, nameof(value));
        }

        var bytes = new byte[16];
        Buffer.BlockCopy(value, 0, bytes, 0, 16);
        SwapByteOrder(bytes);
        return new Guid(bytes);
    }

    /// <summary>
    /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
    /// </summary>
    /// <param name="namespaceId">The ID of the namespace.</param>
    /// <param name="name">The name (within that namespace).</param>
    /// <returns>A UUID derived from the namespace and name.</returns>
    /// <remarks>See <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">Generating a deterministic GUID</a>.</remarks>
    public static Guid NamespaceGuid(Guid namespaceId, string name)
    {
        return NamespaceGuid(namespaceId, name, 5);
    }

    /// <summary>
    /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
    /// </summary>
    /// <param name="namespaceId">The ID of the namespace.</param>
    /// <param name="name">The name (within that namespace).</param>
    /// <returns>A UUID derived from the namespace and name.</returns>
    /// <remarks>See <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">Generating a deterministic GUID</a>.</remarks>
    public static Guid NamespaceGuid(Guid namespaceId, byte[] name)
    {
        return NamespaceGuid(namespaceId, name, 5);
    }

    /// <summary>
    /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
    /// </summary>
    /// <param name="namespaceId">The ID of the namespace.</param>
    /// <param name="name">The name (within that namespace).</param>
    /// <param name="version">The version number of the UUID to create; this value must be either 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
    /// <returns>A UUID derived from the namespace and name.</returns>
    /// <remarks>See <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">Generating a deterministic GUID</a>.</remarks>
    public static Guid NamespaceGuid(Guid namespaceId, string name, int version)
    {
        return NamespaceGuid(namespaceId, Encoding.UTF8.GetBytes(name), version);
    }

    /// <summary>
    /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
    /// </summary>
    /// <param name="namespaceId">The ID of the namespace.</param>
    /// <param name="name">The name (within that namespace).</param>
    /// <param name="version">The version number of the UUID to create; this value must be either 3 (for MD5 hashing) or 5 (for SHA-1 hashing).</param>
    /// <returns>A UUID derived from the namespace and name.</returns>
    /// <remarks>See <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">Generating a deterministic GUID</a>.</remarks>
    [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "Algorithm is used for backwards compatibility.")]
    [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Algorithm is used for backwards compatibility.")]
    public static Guid NamespaceGuid(Guid namespaceId, byte[] name, int version)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (version is not 3 and not 5)
        {
            throw new ArgumentOutOfRangeException(nameof(version), Resources.ArgumentOutOfRangeException_VersionMustBe3Or5);
        }

        var namespaceBytes = namespaceId.ToByteArray();

        SwapByteOrder(namespaceBytes);

        using var algorithm = version == 3 ? (HashAlgorithm)MD5.Create() : SHA1.Create();
        algorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
        algorithm.TransformFinalBlock(name, 0, name.Length);
        var hash = algorithm.Hash;

        var bytes = new byte[16];
        Array.Copy(hash, 0, bytes, 0, 16);

        bytes[6] = (byte)((bytes[6] & 0x0F) | (version << 4));
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        SwapByteOrder(bytes);

        return new Guid(bytes);
    }

    /// <summary>
    /// Return a 16-byte byte array for the specified <see cref="Guid" /> in network byte order (big-endian).
    /// </summary>
    /// <param name="value">The <see cref="Guid" /> to convert.</param>
    /// <returns>A 16-byte byte array for the specified <see cref="Guid" /> in network byte order (big-endian).</returns>
    public static byte[] ToNetworkByteArray(Guid value)
    {
        var bytes = value.ToByteArray();
        SwapByteOrder(bytes);
        return bytes;
    }

    /// <summary>
    /// Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
    /// </summary>
    /// <param name="value">The GUID to convert.</param>
    private static void SwapByteOrder(byte[] value)
    {
        (value[0], value[3]) = (value[3], value[0]);
        (value[1], value[2]) = (value[2], value[1]);
        (value[4], value[5]) = (value[5], value[4]);
        (value[6], value[7]) = (value[7], value[6]);
    }
}