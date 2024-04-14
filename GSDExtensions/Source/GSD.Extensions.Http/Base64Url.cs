// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base64Url.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Http;

using System;
using GSD.Extensions.Http.Properties;

/// <summary>
/// Encodes and decodes strings as Base64url.
/// </summary>
public static class Base64Url
{
    /// <summary>
    /// Encodes the specified string.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>The encoded string.</returns>
    public static byte[] Decode(string value)
    {
        if (value == null)
        {
            return null;
        }

        var s = value
            .Replace('-', '+')
            .Replace('_', '/');

        switch (s.Length % 4)
        {
            case 0: break;
            case 2:
                s += "==";
                break;

            case 3:
                s += "=";
                break;

            default:
                throw new ArgumentException(Resources.ArgumentException_Illegal_Base64Url_String);
        }

        return Convert.FromBase64String(s);
    }

    /// <summary>
    /// Decodes the specified string.
    /// </summary>
    /// <param name="value">THe string to decode.</param>
    /// <returns>The decoded string.</returns>
    public static string Encode(byte[] value)
    {
        if (value == null)
        {
            return null;
        }

        return Convert.ToBase64String(value)
            .Split('=')[0]
            .Replace('+', '-')
            .Replace('/', '_');
    }
}