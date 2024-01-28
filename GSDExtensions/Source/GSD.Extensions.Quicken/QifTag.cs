// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifTag.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

/// <summary>
/// Represents a Quicken tag.
/// </summary>
public class QifTag
{
    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Overrides <see cref="object.ToString" /> to return the name of the category.
    /// </summary>
    /// <returns>The name of the category.</returns>
    public override string ToString()
    {
        return this.Name;
    }
}