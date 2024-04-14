// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationPageBase.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Components;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// Provides a base class for components to prevent services from being called more than once with the same parameters.
/// </summary>
public abstract class ValidationPageBase : ParameterComponentBase
{
    /// <summary>
    /// Gets or sets the edit context.
    /// </summary>
    protected EditContext EditContext { get; set; }

    /// <summary>
    /// Gets or sets the validation message store.
    /// </summary>
    protected ValidationMessageStore MessageStore { get; set; }

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// </summary>
    protected override void OnInitialized()
    {
        this.EditContext = new EditContext(this);
        this.EditContext.OnValidationRequested += this.OnInternalValidationRequested;
        this.EditContext.SetFieldCssClassProvider(new BootstrapFieldCssClassProvider());

        this.MessageStore = new ValidationMessageStore(this.EditContext);

        base.OnInitialized();
    }

    /// <summary>
    /// Occurs when validation is requested.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ValidationRequestedEventArgs" /> that contains the event data.</param>
    protected virtual void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
    }

    /// <summary>
    /// Occurs when validation is requested.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ValidationRequestedEventArgs" /> that contains the event data.</param>
    private void OnInternalValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
        this.MessageStore.Clear();
        this.OnValidationRequested(sender, e);
    }
}