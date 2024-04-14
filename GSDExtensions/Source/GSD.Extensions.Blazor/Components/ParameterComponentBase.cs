// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterComponentBase.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Provides a base class for components to prevent services from being called more than once with the same parameters.
/// </summary>
public abstract class ParameterComponentBase : ComponentBase
{
    /// <summary>
    /// The token to prevent services from being called more than once with the same parameters.
    /// </summary>
    private string parameterToken;

    /// <summary>
    /// Gets the token to prevent services from being called more than once with the same parameters.
    /// </summary>
    protected abstract string ParameterToken { get; }

    /// <summary>
    /// Called once from <see cref="OnParametersSetAsync" /> when the parameters have changed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    protected virtual Task OnParametersChangedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    protected override Task OnParametersSetAsync()
    {
        var token = this.ParameterToken;

        if (token == this.parameterToken)
        {
            return Task.CompletedTask;
        }

        this.parameterToken = token;

        return this.OnParametersChangedAsync();
    }
}