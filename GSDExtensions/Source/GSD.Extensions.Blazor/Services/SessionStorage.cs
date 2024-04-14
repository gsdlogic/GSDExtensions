// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionStorage.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Services;

using Microsoft.JSInterop;

/// <summary>
/// Provides session storage.
/// </summary>
public class SessionStorage
{
    /// <summary>
    /// The JavaScript runtime.
    /// </summary>
    private readonly IJSRuntime jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionStorage" /> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public SessionStorage(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Gets an item from session storage.
    /// </summary>
    /// <param name="key">The key for the item to retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the item from session storage.</returns>
    public async Task<string> GetItemAsync(string key, CancellationToken cancellationToken = default)
    {
        return await this.jsRuntime.InvokeAsync<string>("sessionStorage.getItem", cancellationToken, key).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes an item from session storage.
    /// </summary>
    /// <param name="key">The key for the item to remove.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public async Task RemoveItemAsync(string key, CancellationToken cancellationToken = default)
    {
        await this.jsRuntime.InvokeAsync<object>("sessionStorage.removeItem", cancellationToken, key).ConfigureAwait(false);
    }

    /// <summary>
    /// Stores an item in session storage.
    /// </summary>
    /// <param name="key">The key for the item to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public async Task SetItemAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await this.jsRuntime.InvokeAsync<object>("sessionStorage.setItem", cancellationToken, key, value).ConfigureAwait(false);
    }
}