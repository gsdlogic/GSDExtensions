// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebApiClient.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides a client for a Web API.
/// </summary>
public interface IWebApiClient : IDisposable
{
    /// <summary>
    /// Gets the content serializer.
    /// </summary>
    IContentSerializer Serializer { get; }

    /// <summary>
    /// Gets the access token provider.
    /// </summary>
    IAccessTokenProvider TokenProvider { get; }

    /// <summary>
    /// Sends an HTTP request to the server as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <param name="timeout">The timespan to wait before the request times out.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP response message.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, TimeSpan timeout, CancellationToken cancellationToken = default);
}