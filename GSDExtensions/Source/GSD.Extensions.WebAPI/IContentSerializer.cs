// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContentSerializer.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides methods to serialize and deserialize content for Web API requests.
/// </summary>
public interface IContentSerializer
{
    /// <summary>
    /// Deserializes the response to an HTTP client request.
    /// </summary>
    /// <typeparam name="TContent">The type of object to deserialize the response content.</typeparam>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response content.</returns>
    /// <exception cref="WebApiClientException">An error occurred during JSON deserialization.</exception>
    Task<TContent> DeserializeAsync<TContent>(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes the content for an HTTP client request.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="content">The content to serialize.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP content.</returns>
    Task<HttpContent> SerializeAsync(HttpRequestMessage request, object content, CancellationToken cancellationToken = default);
}