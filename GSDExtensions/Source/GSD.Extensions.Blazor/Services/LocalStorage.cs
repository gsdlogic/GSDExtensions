// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalStorage.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Services;

using Microsoft.JSInterop;

/// <summary>
/// Provides local storage.
/// </summary>
public class LocalStorage
{
    /// <summary>
    /// The JavaScript runtime.
    /// </summary>
    private readonly IJSRuntime jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStorage" /> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public LocalStorage(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Gets an item from local storage.
    /// </summary>
    /// <param name="key">The key for the item to retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the item from local storage.</returns>
    public async Task<string> GetItemAsync(string key, CancellationToken cancellationToken = default)
    {
        return await this.jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, key).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes an item from local storage.
    /// </summary>
    /// <param name="key">The key for the item to remove.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public async Task RemoveItemAsync(string key, CancellationToken cancellationToken = default)
    {
        await this.jsRuntime.InvokeAsync<object>("localStorage.removeItem", cancellationToken, key).ConfigureAwait(false);
    }

    /// <summary>
    /// Stores an item in local storage.
    /// </summary>
    /// <param name="key">The key for the item to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    public async Task SetItemAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await this.jsRuntime.InvokeAsync<object>("localStorage.setItem", cancellationToken, key, value).ConfigureAwait(false);
    }
}