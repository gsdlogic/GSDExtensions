// <copyright file="QueryDispatcher.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Default implementation of <see cref="IQueryDispatcher" /> that resolves query handlers from the active service scope.
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    /// <summary>
    /// The scoped service provider stack used to resolve query handlers.
    /// </summary>
    private readonly IServiceProviderStack serviceProviderStack;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryDispatcher" /> class.
    /// </summary>
    /// <param name="serviceProviderStack">The scoped service provider stack used to resolve query handlers.</param>
    public QueryDispatcher(IServiceProviderStack serviceProviderStack)
    {
        this.serviceProviderStack = serviceProviderStack ?? throw new ArgumentNullException(nameof(serviceProviderStack));
    }

    /// <inheritdoc />
    public async Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var handlerType = typeof(IQueryHandler<TQuery, TResult>);
        var provider = this.serviceProviderStack.Current;

        if (provider.GetService(handlerType) is not IQueryHandler<TQuery, TResult> handler)
        {
            throw new InvalidOperationException($"No query handler registered for type {handlerType.FullName}.");
        }

        return await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
    }
}