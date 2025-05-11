// <copyright file="IQueryHandler.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a handler that processes a query and returns a result.
/// </summary>
/// <typeparam name="TQuery">The type of query.</typeparam>
/// <typeparam name="TResult">The type of result expected from the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult>
{
    /// <summary>
    /// Handles the query and returns the result.
    /// </summary>
    /// <param name="query">The query to process.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the operation, with the result as <typeparamref name="TResult" />.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}