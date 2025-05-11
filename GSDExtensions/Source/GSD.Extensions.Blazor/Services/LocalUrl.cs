// <copyright file="LocalUrl.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Blazor.Services;

/// <summary>
/// Provides methods to determine if a URL is local.
/// </summary>
public static class LocalUrl
{
    /// <summary>
    /// Determines if the specified URL is local.
    /// </summary>
    /// <param name="value">The URL to check.</param>
    /// <returns><see langword="true" /> if the URL is local; otherwise, <see langword="false" />.</returns>
    /// <remarks>
    /// A copy of <see href="https://github.com/dotnet/aspnetcore/blob/3f1acb59718cadf111a0a796681e3d3509bb3381/src/Mvc/Mvc.Core/src/Routing/UrlHelperBase.cs#L315" />.
    /// </remarks>
    public static bool IsLocal(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // Allows "/" or "/foo" but not "//" or "/\".
        if (value[0] == '/')
        {
            // url is exactly "/"
            if (value.Length == 1)
            {
                return true;
            }

            // url doesn't start with "//" or "/\"
            if ((value[1] != '/') && (value[1] != '\\'))
            {
                return !HasControlCharacter(value.AsSpan(1));
            }

            return false;
        }

        // Allows "~/" or "~/foo" but not "~//" or "~/\".
        if ((value[0] == '~') && (value.Length > 1) && (value[1] == '/'))
        {
            // url is exactly "~/"
            if (value.Length == 2)
            {
                return true;
            }

            // url doesn't start with "~//" or "~/\"
            if ((value[2] != '/') && (value[2] != '\\'))
            {
                return !HasControlCharacter(value.AsSpan(2));
            }

            return false;
        }

        return !Uri.TryCreate(value, UriKind.Absolute, out _) &&
               !HasControlCharacter(value);

        static bool HasControlCharacter(ReadOnlySpan<char> readOnlySpan)
        {
            foreach (var c in readOnlySpan)
            {
                if (char.IsControl(c))
                {
                    return true;
                }
            }

            return false;
        }
    }
}