// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifTransaction.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

/// <summary>
/// Represents a Quicken transaction for non-investment accounts.
/// </summary>
public class QifTransaction
{
    /// <summary>
    /// Gets or sets the transaction address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets the transaction split balance.
    /// </summary>
    /// <value>The transaction split balance.</value>
    public decimal Balance => this.Splits.Count == 0 ? this.Amount : this.Splits.Sum(split => split.Amount);

    /// <summary>
    /// Gets or sets the transaction category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction cleared status.
    /// </summary>
    public char Cleared { get; set; } = ' ';

    /// <summary>
    /// Gets or sets the transaction date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the transaction memo.
    /// </summary>
    public string Memo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional message for the transaction address.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check or reference number.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction payee.
    /// </summary>
    public string Payee { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of shares.
    /// </summary>
    public decimal QuantityOfShares { get; set; }

    /// <summary>
    /// Gets or sets the security name.
    /// </summary>
    public string SecurityName { get; set; }

    /// <summary>
    /// Gets or sets the security price.
    /// </summary>
    public decimal SecurityPrice { get; set; }

    /// <summary>
    /// Gets the collection of split items.
    /// </summary>
    public Collection<QifSplit> Splits { get; } = new ();

    /// <summary>
    /// Gets or sets an undocumented value.
    /// </summary>
    public decimal Unknown { get; set; }

    /// <summary>
    /// Overrides <see cref="object.ToString" /> to return the transaction information.
    /// </summary>
    /// <returns>The transaction category and amount.</returns>
    public override string ToString()
    {
        return string.Format(
            CultureInfo.CurrentCulture,
            "{0:MM/dd/yy} {1}, ${2}",
            this.Date,
            this.Payee,
            this.Amount);
    }

    /// <summary>
    /// Ensures the split total matches the transaction amount.
    /// </summary>
    /// <param name="msg">The message to include if an exception is thrown.</param>
    internal void Validate(string msg)
    {
        if (this.Amount != this.Balance)
        {
            throw new InvalidOperationException(msg);
        }
    }
}