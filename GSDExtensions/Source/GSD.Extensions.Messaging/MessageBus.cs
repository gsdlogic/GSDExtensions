// <copyright file="MessageBus.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a unified interface for sending commands, queries, and publishing events.
/// </summary>
public class MessageBus : IMessageBus
{
    /// <summary>
    /// The command dispatcher.
    /// </summary>
    private readonly ICommandDispatcher commandDispatcher;

    /// <summary>
    /// The event publisher.
    /// </summary>
    private readonly IEventPublisher eventPublisher;

    /// <summary>
    /// The query dispatcher.
    /// </summary>
    private readonly IQueryDispatcher queryDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBus" /> class.
    /// </summary>
    /// <param name="commandDispatcher">The command dispatcher.</param>
    /// <param name="queryDispatcher">The query dispatcher.</param>
    /// <param name="eventPublisher">The event publisher.</param>
    public MessageBus(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IEventPublisher eventPublisher)
    {
        this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        this.queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    /// <summary>
    /// Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="eventData">The event instance.</param>
    public void PublishEvent<TEvent>(TEvent eventData)
    {
        this.eventPublisher.PublishEvent(eventData);
    }

    /// <summary>
    /// Sends a message without expecting a response.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendCommandAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        await this.commandDispatcher.SendCommandAsync(message, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a message expecting a response.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response.</returns>
    public async Task<TResult> SendQueryAsync<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
    {
        return await this.queryDispatcher.SendQueryAsync<TMessage, TResult>(message, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Subscribes to an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The event handler delegate.</param>
    public void SubscribeEvent<TEvent>(Action<TEvent> handler)
    {
        this.eventPublisher.SubscribeEvent(handler);
    }

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The event handler delegate to remove.</param>
    public void UnsubscribeEvent<TEvent>(Action<TEvent> handler)
    {
        this.eventPublisher.UnsubscribeEvent(handler);
    }
}