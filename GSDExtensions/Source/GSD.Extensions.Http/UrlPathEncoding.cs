// <copyright file="UrlPathEncoding.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Http;

using System.Text;

/// <summary>
/// Provides methods for encoding and decoding special characters in URL path segments.
/// </summary>
/// <remarks>
/// This class is intended to encode and decode individual segments of a URL path to ensure that special characters
/// are safely transmitted over HTTP. The <see cref="Escape(string)" /> method replaces specific characters in the input string to ensure they are URL-safe:
/// <list type="bullet">
///     <item>
///         <description><c>~</c> is replaced with <c>~0</c></description>
///     </item>
///     <item>
///         <description><c>/</c> is replaced with <c>~1</c></description>
///     </item>
///     <item>
///         <description><c>//</c> is replaced with <c>~2</c></description>
///     </item>
///     <item>
///         <description><c>///</c> is replaced with <c>~3</c></description>
///     </item>
/// </list>
/// The <see cref="Unescape(string)" /> method reverses these replacements to restore the original string:
/// <list type="bullet">
///     <item>
///         <description><c>~</c> is replaced with <c>1</c></description>
///     </item>
///     <item>
///         <description><c>~0</c> is replaced with <c>~</c></description>
///     </item>
///     <item>
///         <description><c>~1</c> is replaced with <c>/</c></description>
///     </item>
///     <item>
///         <description><c>~2</c> is replaced with <c>//</c></description>
///     </item>
///     <item>
///         <description><c>~3</c> is replaced with <c>///</c></description>
///     </item>
/// </list>
/// </remarks>
public static class UrlPathEncoding
{
    /// <summary>
    /// Encodes the specified string by replacing certain characters with URL-safe equivalents.
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>The encoded string with special characters replaced.</returns>
    public static string Escape(string input)
    {
        if (input == null)
        {
            return null;
        }

        var sb = new StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case '~':
                {
                    sb.Append("~0");
                    break;
                }

                case '/':
                {
                    if ((i + 1 < input.Length) && (input[i + 1] == '/'))
                    {
                        if ((i + 2 < input.Length) && (input[i + 2] == '/'))
                        {
                            sb.Append("~3");
                            i += 2;
                        }
                        else
                        {
                            sb.Append("~2");
                            i++;
                        }
                    }
                    else
                    {
                        sb.Append("~1");
                    }

                    break;
                }

                default:
                {
                    sb.Append(input[i]);
                    break;
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Decodes the specified string by reversing the replacements made by the <see cref="Escape(string)" /> method.
    /// </summary>
    /// <param name="input">The string to decode.</param>
    /// <returns>The decoded string with original characters restored.</returns>
    public static string Unescape(string input)
    {
        if (input == null)
        {
            return null;
        }

        var sb = new StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case '~':
                    if (i + 1 < input.Length)
                    {
                        switch (input[i + 1])
                        {
                            case '0':
                                sb.Append('~');
                                i++;
                                break;
                            case '1':
                                sb.Append('/');
                                i++;
                                break;
                            case '2':
                                sb.Append("//");
                                i++;
                                break;
                            case '3':
                                sb.Append("///");
                                i++;
                                break;
                            default:
                                sb.Append('/');
                                break;
                        }
                    }
                    else
                    {
                        sb.Append('/');
                    }

                    break;

                default:
                    sb.Append(input[i]);
                    break;
            }
        }

        return sb.ToString();
    }
}