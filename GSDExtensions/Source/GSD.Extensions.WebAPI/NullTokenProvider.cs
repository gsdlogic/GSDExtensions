// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullTokenProvider.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for obtaining an access token to authorize Web API client requests.
/// </summary>
public class NullTokenProvider : IAccessTokenProvider
{
    /// <summary>
    /// Gets an access token to authorize Web API client requests.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents any asynchronous operation whose result contains the access token, or <see langword="null" /> if the client is not authenticated.</returns>
    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string>(null);
    }
}