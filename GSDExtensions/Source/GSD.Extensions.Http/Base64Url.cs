// <copyright file="Base64Url.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Http;

using System;

/// <summary>
/// Provides methods for encoding and decoding data using Base64url encoding.
/// Base64url is a variation of Base64 encoding that replaces '+' and '/' with '-' and '_',
/// and omits padding characters ('='), commonly used in URLs and web applications.
/// </summary>
public static class Base64Url
{
    /// <summary>
    /// Decodes a Base64url-encoded string into a byte array.
    /// Base64url encoding replaces '+' with '-', '/' with '_', and omits padding characters.
    /// This method handles these variations and performs the decoding.
    /// </summary>
    /// <param name="value">The Base64url-encoded string to decode.</param>
    /// <returns>
    /// A byte array containing the decoded data.
    /// Returns <c>null</c> if the input string is <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the input string is not a valid Base64url-encoded string.
    /// </exception>
    public static byte[] Decode(string value)
    {
        if (value == null)
        {
            return null;
        }

        var base64String = value
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64String.Length % 4)
        {
            case 2:
                base64String += "==";
                break;
            case 3:
                base64String += "=";
                break;
        }

        return Convert.FromBase64String(base64String);
    }

    /// <summary>
    /// Encodes a byte array into a Base64url-encoded string.
    /// Base64url encoding replaces '+' with '-', '/' with '_', and omits padding characters.
    /// This method performs the encoding and returns the encoded string.
    /// </summary>
    /// <param name="value">The byte array to encode.</param>
    /// <returns>
    /// A Base64url-encoded string representing the byte array.
    /// Returns <c>null</c> if the input byte array is <c>null</c>.
    /// </returns>
    public static string Encode(byte[] value)
    {
        if (value == null)
        {
            return null;
        }

        return Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}