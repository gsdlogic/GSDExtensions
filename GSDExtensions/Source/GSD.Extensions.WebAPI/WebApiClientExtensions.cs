// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiClientExtensions.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides base extension methods for the <see cref="IWebApiClient" /> interface.
/// </summary>
public static class WebApiClientBaseExtensions
{
    /// <summary>
    /// Sends an HTTP DELETE request to the server as an asynchronous operation.
    /// </summary>
    /// <param name="client">The Web API client.</param>
    /// <param name="requestPath">A string tha represents the request path.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP response message.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static Task<HttpResponseMessage> DeleteAsync(this IWebApiClient client, string requestPath, string accessToken, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (requestPath == null)
        {
            throw new ArgumentNullException(nameof(requestPath));
        }

        using var request = new HttpRequestMessage(HttpMethod.Delete, requestPath.TrimStart('/'));

        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return client.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The Web API client.</param>
    /// <param name="requestPath">A string tha represents the request path.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static Task<TResponse> GetAsync<TResponse>(this IWebApiClient client, string requestPath, string accessToken, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (requestPath == null)
        {
            throw new ArgumentNullException(nameof(requestPath));
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, requestPath.TrimStart('/'));

        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return client.SendAsync<TResponse>(request, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The Web API client.</param>
    /// <param name="requestPath">A string tha represents the request path.</param>
    /// <param name="content">The contents of the HTTP message.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static Task<TResponse> PostAsync<TResponse>(this IWebApiClient client, string requestPath, object content, string accessToken, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (requestPath == null)
        {
            throw new ArgumentNullException(nameof(requestPath));
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, requestPath.TrimStart('/'));

        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return client.SendAsync<TResponse>(request, content, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The Web API client.</param>
    /// <param name="requestPath">A string tha represents the request path.</param>
    /// <param name="content">The contents of the HTTP message.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static Task<TResponse> PutAsync<TResponse>(this IWebApiClient client, string requestPath, object content, string accessToken, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (requestPath == null)
        {
            throw new ArgumentNullException(nameof(requestPath));
        }

        using var request = new HttpRequestMessage(HttpMethod.Put, requestPath.TrimStart('/'));

        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return client.SendAsync<TResponse>(request, content, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request to the server as an asynchronous operation.
    /// </summary>
    /// <param name="client">The Web API client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP response message.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static Task<HttpResponseMessage> SendAsync(this IWebApiClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        return client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, TimeSpan.FromSeconds(100), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The Web API client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> SendAsync<TResponse>(this IWebApiClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await client.Serializer.DeserializeAsync<TResponse>(request, response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The Web API client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="content">The contents of the HTTP message.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> SendAsync<TResponse>(this IWebApiClient client, HttpRequestMessage request, object content, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        using var requestContent = await client.Serializer.SerializeAsync(request, content, cancellationToken).ConfigureAwait(false);
        request.Content = requestContent;

        return await client.SendAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }
}