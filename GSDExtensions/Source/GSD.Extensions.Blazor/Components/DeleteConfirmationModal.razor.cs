// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteConfirmationModal.razor.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

/// <summary>
/// Contains interaction logic for <c>DeleteConfirmationModal.razor</c>.
/// </summary>
public partial class DeleteConfirmationModal
{
    /// <summary>
    /// Gets or sets the header for the dialog.
    /// </summary>
    [Parameter]
    public string Header { get; set; }

    /// <summary>
    /// Gets or sets the confirmation identifier.
    /// </summary>
    public string Id { get; set; } = nameof(DeleteConfirmationModal);

    /// <summary>
    /// Gets or sets the message to display to the user.
    /// </summary>
    [Parameter]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the event callback to invoke when the user clicks the delete button.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnDelete { get; set; }
}