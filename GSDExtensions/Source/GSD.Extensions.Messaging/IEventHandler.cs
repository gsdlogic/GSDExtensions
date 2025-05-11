// <copyright file="IEventHandler.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a handler for processing a specific event type.
/// </summary>
/// <typeparam name="TEvent">The type of event this handler processes.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "IEventHandler<T> follows the naming convention for CQRS handlers.")]
public interface IEventHandler<in TEvent>
{
    /// <summary>
    /// Handles the specified event.
    /// </summary>
    /// <param name="eventData">The event to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TEvent eventData, CancellationToken cancellationToken = default);
}