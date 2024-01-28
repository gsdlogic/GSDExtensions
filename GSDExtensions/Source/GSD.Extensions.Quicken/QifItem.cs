// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifItem.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

/// <summary>
/// Represents a line item in a QIF file.
/// </summary>
public class QifItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QifItem" /> class.
    /// </summary>
    /// <param name="text">The item text.</param>
    public QifItem(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            this.EndOfFile = true;
            this.EndOfRecord = true;
        }
        else
        {
            this.Field = text[0];
            this.Value = text[1..].Trim();
            this.Text = text;
            this.EndOfRecord = this.Field == '!';
        }
    }

    /// <summary>
    /// Gets a value indicating whether the end of file has been reached.
    /// </summary>
    public bool EndOfFile { get; }

    /// <summary>
    /// Gets a value indicating whether the end of record has been reached.
    /// </summary>
    public bool EndOfRecord { get; }

    /// <summary>
    /// Gets the item field.
    /// </summary>
    public char Field { get; }

    /// <summary>
    /// Gets the item text.
    /// </summary>
    public string Text { get; } = string.Empty;

    /// <summary>
    /// Gets the item value.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Overrides <see cref="object.ToString" /> to return the item text.
    /// </summary>
    /// <returns>The item text.</returns>
    public override string ToString()
    {
        return this.Text;
    }
}