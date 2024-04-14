// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base64UrlString.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Http;

using System.Text;

/// <summary>
/// Encodes and decodes strings as Base64url.
/// </summary>
public static class Base64UrlString
{
    /// <summary>
    /// Encodes the specified string.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>The encoded string.</returns>
    public static string Decode(string value)
    {
        return value == null ? null : Encoding.UTF8.GetString(Base64Url.Decode(value));
    }

    /// <summary>
    /// Decodes the specified string.
    /// </summary>
    /// <param name="value">THe string to decode.</param>
    /// <returns>The decoded string.</returns>
    public static string Encode(string value)
    {
        return value == null ? null : Base64Url.Encode(Encoding.UTF8.GetBytes(value));
    }
}