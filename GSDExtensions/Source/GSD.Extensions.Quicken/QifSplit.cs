// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifSplit.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

using System.Globalization;

/// <summary>
/// Represents a split transaction.
/// </summary>
public class QifSplit
{
    /// <summary>
    /// Gets or sets the split amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the category for the split.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the memo for the split.
    /// </summary>
    public string Memo { get; set; }

    /// <summary>
    /// Overrides <see cref="object.ToString" /> to return the transaction information.
    /// </summary>
    /// <returns>The transaction category and amount.</returns>
    public override string ToString()
    {
        return string.Format(
            CultureInfo.CurrentCulture,
            "{0}, ${1}",
            this.Category,
            this.Amount);
    }
}