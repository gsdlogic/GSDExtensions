// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientException.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Http;

using System;
using System.Runtime.Serialization;
using GSD.Extensions.Http.Properties;

/// <summary>
/// Represents errors that occur during HTTP client requests.
/// </summary>
[Serializable]
public class HttpClientException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class.
    /// </summary>
    public HttpClientException()
        : base(Resources.HttpClientException_DefaultMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with a specified error message..
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public HttpClientException(string message)
        : base(message ?? Resources.HttpClientException_DefaultMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null" /> if no inner exception is specified.</param>
    public HttpClientException(string message, Exception innerException)
        : base(message ?? Resources.HttpClientException_DefaultMessage, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with a specified error message..
    /// </summary>
    /// <param name="httpMethod">The HTTP method for the request.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="message">The message that describes the error.</param>
    public HttpClientException(string httpMethod, Uri requestUri, string message)
        : base(message ?? Resources.HttpClientException_DefaultMessage)
    {
        this.HttpMethod = httpMethod;
        this.RequestUri = requestUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="httpMethod">The HTTP method for the request.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null" /> if no inner exception is specified.</param>
    public HttpClientException(string httpMethod, Uri requestUri, string message, Exception innerException)
        : base(message ?? Resources.HttpClientException_DefaultMessage, innerException)
    {
        this.HttpMethod = httpMethod;
        this.RequestUri = requestUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="httpMethod">The HTTP method for the request.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="statusCode">The status code of the HTTP response.</param>
    /// <param name="reasonPhrase">The reason phrase which typically is sent by servers together with the status code.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null" /> if no inner exception is specified.</param>
    public HttpClientException(string httpMethod, Uri requestUri, int statusCode, string reasonPhrase, string message, Exception innerException)
        : base(message ?? Resources.HttpClientException_DefaultMessage, innerException)
    {
        this.HttpMethod = httpMethod;
        this.RequestUri = requestUri;
        this.StatusCode = statusCode;
        this.ReasonPhrase = reasonPhrase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with a specified error returned from the remote server.
    /// </summary>
    /// <param name="httpMethod">The HTTP method for the request.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="statusCode">The status code of the HTTP response.</param>
    /// <param name="reasonPhrase">The reason phrase which typically is sent by servers together with the status code.</param>
    /// <param name="responseBody">The HTTP response body.</param>
    public HttpClientException(string httpMethod, Uri requestUri, int statusCode, string reasonPhrase, string responseBody)
        : base($"{Resources.HttpClientException_DefaultMessage} ({statusCode} {reasonPhrase})")
    {
        this.HttpMethod = httpMethod;
        this.RequestUri = requestUri;
        this.StatusCode = statusCode;
        this.ReasonPhrase = reasonPhrase;
        this.ResponseBody = responseBody;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientException" /> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    protected HttpClientException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Gets the HTTP method for the request.
    /// </summary>
    public string HttpMethod { get; }

    /// <summary>
    /// Gets the reason phrase which typically is sent by servers together with the status code.
    /// </summary>
    public string ReasonPhrase { get; }

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public Uri RequestUri { get; }

    /// <summary>
    /// Gets the HTTP response body.
    /// </summary>
    public string ResponseBody { get; }

    /// <summary>
    /// Gets the status code of the HTTP response.
    /// </summary>
    public int StatusCode { get; }
}