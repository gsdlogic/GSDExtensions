// <copyright file="EventPublisher.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines a unified interface for publishing events.
/// </summary>
public class EventPublisher : IEventPublisher
{
    /// <summary>
    /// The collection of event handlers.
    /// </summary>
    private readonly ConcurrentDictionary<Type, List<WeakReference>> eventHandlers = new();

    /// <summary>
    /// Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="eventData">The event instance.</param>
    public void PublishEvent<TEvent>(TEvent eventData)
    {
        var type = typeof(TEvent);

        if (!this.eventHandlers.TryGetValue(type, out var handlers))
        {
            return;
        }

        foreach (var reference in handlers.ToList())
        {
            if (reference.Target is Action<TEvent> handler)
            {
                handler(eventData);
            }
            else
            {
                handlers.Remove(reference);
            }
        }
    }

    /// <summary>
    /// Subscribes to an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The event handler delegate.</param>
    public void SubscribeEvent<TEvent>(Action<TEvent> handler)
    {
        var type = typeof(TEvent);

        if (!this.eventHandlers.TryGetValue(type, out var handlers))
        {
            handlers = [];
            this.eventHandlers[type] = handlers;
        }

        handlers.Add(new WeakReference(handler));
    }

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The event handler delegate to remove.</param>
    public void UnsubscribeEvent<TEvent>(Action<TEvent> handler)
    {
        var type = typeof(TEvent);

        if (this.eventHandlers.TryGetValue(type, out var handlers))
        {
            handlers.RemoveAll(wr => wr.Target is Action<TEvent> h && (h == handler));
        }
    }
}