// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiClient.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides a client for a Web API.
/// </summary>
public class WebApiClient : IWebApiClient
{
    /// <summary>
    /// The HTTP client.
    /// </summary>
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClient" /> class.
    /// </summary>
    /// <param name="settings">The settings for the Web API client.</param>
    public WebApiClient(WebApiClientSettings settings)
        : this(new NullTokenProvider(), settings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClient" /> class.
    /// </summary>
    /// <param name="tokenProvider">The access token provider.</param>
    /// <param name="settings">The settings for the Web API client.</param>
    public WebApiClient(IAccessTokenProvider tokenProvider, WebApiClientSettings settings)
        : this(new JsonContentSerializer(), tokenProvider, settings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiClient" /> class.
    /// </summary>
    /// <param name="serializer">The content serializer.</param>
    /// <param name="tokenProvider">The access token provider.</param>
    /// <param name="settings">The settings for the Web API client.</param>
    public WebApiClient(IContentSerializer serializer, IAccessTokenProvider tokenProvider, WebApiClientSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        this.Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        this.TokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));

        this.client = new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan,
            BaseAddress = settings.BaseAddress == null ? null : new Uri($"{settings.BaseAddress.TrimEnd('/')}/"),
        };

        var assemblyName = typeof(WebApiClient).Assembly.GetName();

        this.client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString()));
        this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="WebApiClient" /> class.
    /// </summary>
    ~WebApiClient()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Gets the content serializer.
    /// </summary>
    public IContentSerializer Serializer { get; }

    /// <summary>
    /// Gets the access token provider.
    /// </summary>
    public IAccessTokenProvider TokenProvider { get; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sends an HTTP request to the server as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <param name="timeout">The timespan to wait before the request times out.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP response message.</returns>
    /// <exception cref="WebApiClientException">An error occurred during the request.</exception>
    public virtual async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            HttpResponseMessage response;

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                response = await this.client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSource.CancelAfter(timeout);

                try
                {
                    response = await this.client.SendAsync(request, completionOption, cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
                {
                    throw new TimeoutException();
                }
            }

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.Dispose();

            throw new WebApiClientException(request.Method.Method, request.RequestUri, (int)response.StatusCode, response.ReasonPhrase, responseBody);
        }
        catch (Exception ex) when (ex is
            HttpRequestException or
            SocketException or
            TimeoutException)
        {
            throw new WebApiClientException(request.Method.Method, request.RequestUri, ex.Message, ex);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Indicates whether managed resources will be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        this.client.Dispose();
    }
}