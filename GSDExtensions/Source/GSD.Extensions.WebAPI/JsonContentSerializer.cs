// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonContentSerializer.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI;

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// Provides methods to serialize and deserialize content for Web API requests.
/// </summary>
public class JsonContentSerializer : IContentSerializer
{
    /// <summary>
    /// Gets the JSON serializer.
    /// </summary>
    public JsonSerializer JsonSerializer { get; } = new ();

    /// <summary>
    /// Deserializes the response to an HTTP client request.
    /// </summary>
    /// <typeparam name="TContent">The type of object to deserialize the response content.</typeparam>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the deserialized response content.</returns>
    /// <exception cref="WebApiClientException">An error occurred during JSON deserialization.</exception>
    public async Task<TContent> DeserializeAsync<TContent>(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (response == null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        try
        {
            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            await using var responseStreamDisposable = responseStream.ConfigureAwait(false);

            using var streamReader = new StreamReader(responseStream);
            using var jsonReader = new JsonTextReader(streamReader);

            var result = this.JsonSerializer.Deserialize<TContent>(jsonReader);

            return result;
        }
        catch (JsonException ex)
        {
            throw new WebApiClientException(request.Method.Method, request.RequestUri, (int)response.StatusCode, response.ReasonPhrase, ex.Message, ex);
        }
    }

    /// <summary>
    /// Serializes the content for an HTTP client request.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="content">The content to serialize.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation request.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result contains the HTTP content.</returns>
    public Task<HttpContent> SerializeAsync(HttpRequestMessage request, object content, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);

            this.JsonSerializer.Serialize(jsonWriter, content);

            var jsonContent = stringWriter.ToString();
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return Task.FromResult<HttpContent>(stringContent);
        }
        catch (JsonException ex)
        {
            throw new WebApiClientException(request.Method.Method, request.RequestUri, ex.Message, ex);
        }
    }
}