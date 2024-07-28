// <copyright file="HttpClientExtensions.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Http;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSD.Extensions.Http.Properties;
using Newtonsoft.Json;

/// <summary>
/// Provides extension methods for the <see cref="HttpClient" /> class.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// The JSON serializer.
    /// </summary>
    private static readonly JsonSerializer JsonSerializer = new ();

    /// <summary>
    /// Sends an HTTP DELETE request to the server as an asynchronous operation.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestPath">A string that represents the request path.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP response message.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestPath, string accessToken, CancellationToken cancellationToken = default)
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

        return client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, TimeSpan.FromSeconds(100), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestPath">A string that represents the request path.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> GetAsync<TResponse>(this HttpClient client, string requestPath, string accessToken, CancellationToken cancellationToken = default)
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

        return await client.SendAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP POST request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestPath">A string that represents the request path.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> PostAsync<TResponse>(this HttpClient client, string requestPath, string accessToken, CancellationToken cancellationToken = default)
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

        return await client.SendAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP POST request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestPath">A string that represents the request path.</param>
    /// <param name="content">The contents of the HTTP message.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> PostAsync<TResponse>(this HttpClient client, string requestPath, object content, string accessToken, CancellationToken cancellationToken = default)
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

        return await client.SendAsync<TResponse>(request, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP PUT request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestPath">A string that represents the request path.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> PutAsync<TResponse>(this HttpClient client, string requestPath, string accessToken, CancellationToken cancellationToken = default)
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

        return await client.SendAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP PUT request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestPath">A string that represents the request path.</param>
    /// <param name="content">The contents of the HTTP message.</param>
    /// <param name="accessToken">The access token to authorize the request, or <see langword="null" /> if the request does not require authorization.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> PutAsync<TResponse>(this HttpClient client, string requestPath, object content, string accessToken, CancellationToken cancellationToken = default)
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

        return await client.SendAsync<TResponse>(request, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request with the specified content to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="content">The contents of the HTTP message.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> SendAsync<TResponse>(this HttpClient client, HttpRequestMessage request, object content, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            var json = JsonConvert.SerializeObject(content);
            using var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = requestContent;

            return await client.SendAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            throw new HttpClientException(request.Method.Method, request.RequestUri, ex.Message, ex);
        }
    }

    /// <summary>
    /// Sends an HTTP request to the server as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of object to deserialize the response content.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<TResponse> SendAsync<TResponse>(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, TimeSpan.FromSeconds(100), cancellationToken).ConfigureAwait(false);

        try
        {
            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            await using var responseStreamDisposable = responseStream.ConfigureAwait(false);

            using var streamReader = new StreamReader(responseStream);
            using var jsonReader = new JsonTextReader(streamReader);

            var result = JsonSerializer.Deserialize<TResponse>(jsonReader);

            return result;
        }
        catch (JsonException ex)
        {
            throw new HttpClientException(request.Method.Method, request.RequestUri, (int)response.StatusCode, response.ReasonPhrase, ex.Message, ex);
        }
    }

    /// <summary>
    /// Sends an HTTP request to the server as an asynchronous operation.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <param name="timeout">The timespan to wait before the request times out.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP response message.</returns>
    /// <exception cref="HttpClientException">An error occurred during the request.</exception>
    public static async Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request, HttpCompletionOption completionOption, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            if (client.Timeout != Timeout.InfiniteTimeSpan)
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new HttpClientException(request.Method.Method, request.RequestUri, Resources.HttpClientException_NotFirstRequest, ex);
        }

        try
        {
            HttpResponseMessage response;

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                response = await client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSource.CancelAfter(timeout);

                try
                {
                    response = await client.SendAsync(request, completionOption, cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
                {
                    throw new TimeoutException(Resources.TimeoutException_TheRequestTimedOut);
                }
            }

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.Dispose();

            throw new HttpClientException(request.Method.Method, request.RequestUri, (int)response.StatusCode, response.ReasonPhrase, responseBody);
        }
        catch (Exception ex) when (ex is
            HttpRequestException or
            SocketException or
            TimeoutException)
        {
            throw new HttpClientException(request.Method.Method, request.RequestUri, ex.Message, ex);
        }
    }
}