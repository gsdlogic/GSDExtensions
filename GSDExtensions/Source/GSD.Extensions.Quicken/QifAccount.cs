// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifAccount.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

using System;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Represents a Quicken account.
/// </summary>
public class QifAccount
{
    /// <summary>
    /// Gets or sets the type of account.
    /// </summary>
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Gets the account transaction balance.
    /// </summary>
    /// <value>The account transaction balance.</value>
    public decimal Balance => this.Transactions.Sum(transaction => transaction.Amount);

    /// <summary>
    /// Gets or sets the statement balance date.
    /// </summary>
    public DateTime BalanceDate { get; set; }

    /// <summary>
    /// Gets or sets the credit limit (for credit accounts).
    /// </summary>
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// Gets or sets the account description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the account.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the account balance.
    /// </summary>
    public decimal StatementBalance { get; set; }

    /// <summary>
    /// Gets the list of transactions.
    /// </summary>
    public Collection<QifTransaction> Transactions { get; } = new ();

    /// <summary>
    /// Overrides <see cref="object.ToString" /> to return the name of the account.
    /// </summary>
    /// <returns>The name of the account.</returns>
    public override string ToString()
    {
        return $"{this.Name}: {this.Balance}";
    }
}