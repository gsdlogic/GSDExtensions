// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapFieldCssClassProvider.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Components;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// Provides custom css class names for fields using bootstrap.
/// </summary>
public class BootstrapFieldCssClassProvider : FieldCssClassProvider
{
    /// <summary>
    /// Gets a string that indicates the status of the specified field as a CSS class.
    /// </summary>
    /// <param name="editContext">The <see cref="EditContext" />.</param>
    /// <param name="fieldIdentifier">The <see cref="FieldIdentifier" />.</param>
    /// <returns>A CSS class name string.</returns>
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        ArgumentNullException.ThrowIfNull(editContext);

        return editContext.GetValidationMessages(fieldIdentifier).Any() ? "is-invalid" : string.Empty;
    }
}