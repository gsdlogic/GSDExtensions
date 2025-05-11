// <copyright file="StateObserver.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Notifies subscribers when a model changes.
/// </summary>
public class StateObserver
{
    /// <summary>
    /// The dictionary mapping models to subscribers.
    /// </summary>
    private readonly Dictionary<object, List<SubscriberData>> modelSubscribers = [];

    /// <summary>
    /// The dictionary mapping subscribers to models.
    /// </summary>
    private readonly Dictionary<object, List<object>> subscriberModels = [];

    /// <summary>
    /// Notifies subscribers that the state of a model has changed.
    /// </summary>
    /// <typeparam name="T">The type of model.</typeparam>
    /// <param name="model">The model whose state has changed.</param>
    public void NotifyStateChanged<T>(T model)
    {
        if (!this.modelSubscribers.TryGetValue(model, out var subscribers))
        {
            return;
        }

        foreach (var subscriber in subscribers)
        {
            subscriber.Handler.Invoke();
        }
    }

    /// <summary>
    /// Subscribes to model changes.
    /// </summary>
    /// <typeparam name="T">The type of model.</typeparam>
    /// <param name="subscriber">The subscriber.</param>
    /// <param name="model">The model to observe.</param>
    /// <param name="handler">The delegate to invoke when the state of the model changes.</param>
    public void Subscribe<T>(object subscriber, T model, Action handler)
    {
        if (!this.modelSubscribers.TryGetValue(model, out var subscribers))
        {
            subscribers = [];
            this.modelSubscribers[model] = subscribers;
        }

        if (!this.subscriberModels.TryGetValue(subscriber, out var models))
        {
            models = [];
            this.subscriberModels[subscriber] = models;
        }

        var subscriberData = new SubscriberData(subscriber, handler);

        subscribers.Add(subscriberData);
        models.Add(model);
    }

    /// <summary>
    /// Subscribes to model changes and unsubscribes from changes to any other model.
    /// </summary>
    /// <typeparam name="T">The type of model.</typeparam>
    /// <param name="subscriber">The subscriber.</param>
    /// <param name="model">The model to observe.</param>
    /// <param name="handler">The delegate to invoke when the state of the model changes.</param>
    public void SubscribeSingle<T>(object subscriber, T model, Action handler)
    {
        this.UnsubscribeAll(subscriber);
        this.Subscribe(subscriber, model, handler);
    }

    /// <summary>
    /// Unsubscribes from model changes.
    /// </summary>
    /// <typeparam name="T">The type of model.</typeparam>
    /// <param name="subscriber">The subscriber.</param>
    /// <param name="model">The model to observe.</param>
    public void Unsubscribe<T>(object subscriber, T model)
    {
        if (!this.subscriberModels.TryGetValue(subscriber, out var models))
        {
            return;
        }

        models.Remove(model);

        if (models.Count == 0)
        {
            this.subscriberModels.Remove(subscriber);
        }

        if (!this.modelSubscribers.TryGetValue(model, out var subscribers))
        {
            return;
        }

        var subscriberData = subscribers.FirstOrDefault(s => s.Handle == subscriber);

        if (subscriberData == null)
        {
            return;
        }

        subscribers.Remove(subscriberData);

        if (subscribers.Count == 0)
        {
            this.modelSubscribers.Remove(model);
        }
    }

    /// <summary>
    /// Unsubscribes from all model changes.
    /// </summary>
    /// <param name="subscriber">The subscriber.</param>
    public void UnsubscribeAll(object subscriber)
    {
        if (!this.subscriberModels.TryGetValue(subscriber, out var models))
        {
            return;
        }

        foreach (var model in models.ToArray())
        {
            this.Unsubscribe(subscriber, model);
        }
    }

    /// <summary>
    /// Represents a subscriber.
    /// </summary>
    private sealed class SubscriberData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberData" /> class.
        /// </summary>
        /// <param name="handle">The handle to the subscriber.</param>
        /// <param name="handler">The event handler.</param>
        public SubscriberData(object handle, Action handler)
        {
            this.Handle = handle;
            this.Handler = handler;
        }

        /// <summary>
        /// Gets the handle to the subscriber.
        /// </summary>
        public object Handle { get; }

        /// <summary>
        /// Gets the event handler.
        /// </summary>
        public Action Handler { get; }
    }
}