// <copyright file="IEventPublisher.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;

/// <summary>
/// Defines a unified interface for publishing events.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="eventData">The event instance.</param>
    void PublishEvent<TEvent>(TEvent eventData);

    /// <summary>
    /// Subscribes to an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The event handler delegate.</param>
    void SubscribeEvent<TEvent>(Action<TEvent> handler);

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The event handler delegate to remove.</param>
    void UnsubscribeEvent<TEvent>(Action<TEvent> handler);
}