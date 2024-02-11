// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Soundex.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.DataFormats;

using System.Collections.Generic;
using System.Text;

/// <summary>
/// Provides methods to encode and compare strings based on the way the word is pronounced using the Soundex algorithm.
/// </summary>
/// <remarks>
/// For more information, see https://en.wikipedia.org/wiki/Soundex.
/// </remarks>
public static class Soundex
{
    /// <summary>
    /// The key used to generate the Soundex value.
    /// </summary>
    private static readonly Dictionary<char, char> Key = new ()
    {
        { 'B', '1' },
        { 'F', '1' },
        { 'P', '1' },
        { 'V', '1' },
        { 'C', '2' },
        { 'G', '2' },
        { 'J', '2' },
        { 'K', '2' },
        { 'Q', '2' },
        { 'S', '2' },
        { 'X', '2' },
        { 'Z', '2' },
        { 'D', '3' },
        { 'T', '3' },
        { 'L', '4' },
        { 'M', '5' },
        { 'N', '5' },
        { 'R', '6' },
    };

    /// <summary>
    /// Compares two Soundex encoded values and returns a value indicating the similarity between the words.
    /// </summary>
    /// <param name="soundex1">The first Soundex encoded value to compare.</param>
    /// <param name="soundex2">The second Soundex encoded value to compare.</param>
    /// <returns>A value between 1 (no match) and 4 (perfect match) indicating the similarity between the words.</returns>
    public static int Compare(string soundex1, string soundex2)
    {
        if (string.IsNullOrEmpty(soundex1) || string.IsNullOrEmpty(soundex2))
        {
            return 1;
        }

        var rank = 0;
        var i = 0;
        var j = 0;

        while ((i < soundex1.Length) && (j < soundex2.Length))
        {
            if (soundex1[i] == soundex2[j])
            {
                rank++;
                i++;

                if (j < soundex2.Length - 1)
                {
                    j++;
                }
                else
                {
                    j = 1;
                }
            }
            else if (j < soundex2.Length - 1)
            {
                if (i == 0)
                {
                    i = 1;
                }

                j++;
            }
            else
            {
                i++;
                j = 1;
            }
        }

        return rank;
    }

    /// <summary>
    /// Compares two word values and returns a value indicating the similarity between the words using the encoded Soundex algorithm.
    /// </summary>
    /// <param name="word1">The first word to compare.</param>
    /// <param name="word2">The second word to compare.</param>
    /// <returns>A value between 1 (no match) and 4 (perfect match) indicating the similarity between the words.</returns>
    public static int Difference(string word1, string word2)
    {
        return Compare(Encode(word1), Encode(word2));
    }

    /// <summary>
    /// Encodes a word using the Soundex algorithm based on the way the word is pronounced.
    /// </summary>
    /// <param name="word">The word to encode.</param>
    /// <returns>The encoded Soundex value for the specified value.</returns>
    public static string Encode(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return "0000";
        }

        var result = new StringBuilder();
        var previousCode = '\0';

        foreach (var c in word)
        {
            var key = char.ToUpperInvariant(c);
            char code;

            if (result.Length == 0)
            {
                result.Append(key);

                if (Key.TryGetValue(key, out code))
                {
                    previousCode = code;
                }
            }
            else if (Key.TryGetValue(key, out code) && (code != previousCode))
            {
                result.Append(code);
                previousCode = code;
            }
            else if (key != 'H')
            {
                previousCode = '\0';
            }

            if (result.Length == 4)
            {
                return result.ToString();
            }
        }

        result.Append(new string('0', 4 - result.Length));
        return result.ToString();
    }
}