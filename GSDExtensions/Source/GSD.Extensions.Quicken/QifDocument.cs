// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifDocument.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Represents a QIF file.
/// </summary>
public class QifDocument
{
    /// <summary>
    /// Gets the collection of accounts.
    /// </summary>
    public Collection<QifAccount> Accounts { get; } = new ();

    /// <summary>
    /// Gets balance for all accounts.
    /// </summary>
    /// <value>The documentation account balance.</value>
    public decimal Balance => this.Accounts.Sum(account => account.Balance);

    /// <summary>
    /// Gets the collection of categories.
    /// </summary>
    public Collection<QifCategory> Categories { get; } = new ();

    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    public Collection<QifTag> Tags { get; } = new ();
}