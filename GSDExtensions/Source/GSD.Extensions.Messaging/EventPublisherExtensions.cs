// <copyright file="EventPublisherExtensions.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides extension methods for the <see cref="IEventPublisher" /> interface.
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Subscribes to an event retaining a reference to the event handler delegate.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="retainedHandlers">The collection of references to the event handler delegates.</param>
    /// <param name="handler">The event handler delegate.</param>
    public static void SubscribeEvent<TEvent>(this IEventPublisher eventPublisher, IList<object> retainedHandlers, Action<TEvent> handler)
    {
        if (eventPublisher == null)
        {
            throw new ArgumentNullException(nameof(eventPublisher));
        }

        if (retainedHandlers == null)
        {
            throw new ArgumentNullException(nameof(retainedHandlers));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        retainedHandlers.Add(handler);
        eventPublisher.SubscribeEvent(handler);
    }

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="retainedHandlers">The collection of references to the event handler delegates.</param>
    /// <param name="handler">The event handler delegate to remove.</param>
    public static void UnsubscribeEvent<TEvent>(this IMessageBus eventPublisher, IList<object> retainedHandlers, Action<TEvent> handler)
    {
        if (eventPublisher == null)
        {
            throw new ArgumentNullException(nameof(eventPublisher));
        }

        if (retainedHandlers == null)
        {
            throw new ArgumentNullException(nameof(retainedHandlers));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        retainedHandlers.Remove(handler);
        eventPublisher.UnsubscribeEvent(handler);
    }
}