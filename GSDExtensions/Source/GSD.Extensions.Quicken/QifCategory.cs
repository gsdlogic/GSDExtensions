// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifCategory.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

/// <summary>
/// Represents a Quicken category.
/// </summary>
public class QifCategory
{
    /// <summary>
    /// Gets or sets the category budget amount (only in a Budget Amounts QIF file).
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// Gets or sets the category description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the category is an income category.
    /// </summary>
    public bool IsIncome { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the category is tax related.
    /// </summary>
    public bool TaxRelated { get; set; }

    /// <summary>
    /// Gets or sets the category tax schedule information.
    /// </summary>
    public string TaxSchedule { get; set; } = string.Empty;

    /// <summary>
    /// Overrides <see cref="object.ToString" /> to return the name of the category.
    /// </summary>
    /// <returns>The name of the category.</returns>
    public override string ToString()
    {
        return this.Name;
    }
}