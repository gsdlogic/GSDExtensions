// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidUtilityTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="GuidUtility" /> class.
/// </summary>
public static class GuidUtilityTests
{
    /// <summary>
    /// Ensures that a UUID can be converted to a network byte order (big-endian) array.
    /// </summary>
    [Fact]
    public static void CreatesGuidFromNetworkByteOrderArrays()
    {
        Assert.Equal(
            new Guid("00010203-0405-0607-0809-0a0b0c0d0e0f"),
            GuidUtility.FromNetworkByteArray(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f }));
    }

    /// <summary>
    /// Ensures that name based UUID is generated properly.
    /// </summary>
    [Fact]
    public static void GeneratesNameBasedGuid()
    {
        Assert.Equal(
            new Guid("21f7f8de-8051-5b89-8680-0195ef798b6a"),
            GuidUtility.NamespaceGuid(GuidUtility.DnsNamespace, "www.widgets.com"));
    }

    /// <summary>
    /// Ensures that a UUID can be converted to a network byte order (big-endian) array.
    /// </summary>
    [Fact]
    public static void GeneratesNetworkByteOrderArrays()
    {
        Assert.Equal(
            new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f },
            GuidUtility.ToNetworkByteArray(new Guid("00010203-0405-0607-0809-0a0b0c0d0e0f")));
    }

    /// <summary>
    /// Ensures that the UUID for standard namespaces are provides.
    /// </summary>
    [Fact]
    public static void ProvidesStandardNamespaces()
    {
        Assert.Equal(new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"), GuidUtility.DnsNamespace);
        Assert.Equal(new Guid("6ba7b811-9dad-11d1-80b4-00c04fd430c8"), GuidUtility.UrlNamespace);
        Assert.Equal(new Guid("6ba7b812-9dad-11d1-80b4-00c04fd430c8"), GuidUtility.OidNamespace);
        Assert.Equal(new Guid("6ba7b814-9dad-11d1-80b4-00c04fd430c8"), GuidUtility.X500Namespace);
    }
}