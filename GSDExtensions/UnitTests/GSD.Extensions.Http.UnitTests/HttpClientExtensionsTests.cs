// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientExtensionsTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Http.UnitTests;

using System.Net;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="HttpClientExtensions" /> class.
/// </summary>
public class HttpClientExtensionsTests
{
    /// <summary>
    /// Ensures that the extension methods can deserialize a string token.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task GetDeserializesStringTokensAsync()
    {
        const string RequestPath = "/api/test";
        const string AccessToken = "access_token";

        static HttpResponseMessage HandleRequest(HttpRequestMessage request)
        {
            Assert.Equal(RequestPath, request.RequestUri!.LocalPath);
            Assert.Equal($"Bearer {AccessToken}", request.Headers.Authorization!.ToString());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("\"Hello world!\""),
            };
        }

        using var handler = new TestHandler(HandleRequest);
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://example.com"),
        };

        var response = await client.GetAsync<string>(RequestPath, AccessToken).ConfigureAwait(true);

        Assert.Equal("Hello world!", response);
    }

    /// <summary>
    /// Ensures that the extension methods can deserialize a string token.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task SendThrowsExceptionIfNotFirstRequestAsync()
    {
        static HttpResponseMessage HandleRequest(HttpRequestMessage request)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        using var handler = new TestHandler(HandleRequest);
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://example.com"),
        };

        // Call third party extension before setting timeout property.
        await client.GetStringAsync(new Uri("/api/test", UriKind.Relative)).ConfigureAwait(true);

        // Call extension method that sets timeout property.
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
        var exception = await Assert.ThrowsAsync<HttpClientException>(async () => { await client.GetAsync<string>("api/test", "access_token").ConfigureAwait(true); }).ConfigureAwait(true);
        Assert.Equal("Timeout property can only be modified before sending the first request.", exception.Message);
    }

    /// <summary>
    /// A <see cref="HttpMessageHandler" /> to use for testing.
    /// </summary>
    private sealed class TestHandler : HttpMessageHandler
    {
        /// <summary>
        /// The handler delegate.
        /// </summary>
        private readonly Func<HttpRequestMessage, HttpResponseMessage> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHandler" /> class.
        /// </summary>
        /// <param name="handler">The handler delegate.</param>
        public TestHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.handler.Invoke(request));
        }
    }
}