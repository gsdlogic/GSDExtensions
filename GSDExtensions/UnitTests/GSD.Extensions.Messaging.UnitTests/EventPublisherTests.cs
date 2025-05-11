// <copyright file="EventPublisherTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging.UnitTests;

/// <summary>
/// Unit tests for the EventPublisher class.
/// </summary>
public class EventPublisherTests
{
    /// <summary>
    /// Verifies that event handlers are garbage collected when they go out of scope.
    /// </summary>
    [Fact]
    public void PublishEventAfterSubscriberGoesOutOfScopeDoesNotInvokeHandler()
    {
        var publisher = new EventPublisher();
        var list = new List<string>();

        for (var i = 0; i < 1; i++)
        {
            _ = new TestSubscriber(publisher, s => list.Add(s));
            publisher.PublishEvent("test1");
        }

        GC.Collect();
        publisher.PublishEvent("test2");

        Assert.Single(list);
        Assert.Equal("test1", list[0]);
    }

    /// <summary>
    /// Verifies that the test subscriber handler is invoked when not garbage collected.
    /// </summary>
    [Fact]
    public void PublishEventBeforeSubscriberGoesOutOfScopeInvokesHandler()
    {
        var publisher = new EventPublisher();
        var list = new List<string>();

        _ = new TestSubscriber(publisher, s => list.Add(s));
        publisher.PublishEvent("test1");

        Assert.Single(list);
        Assert.Equal("test1", list[0]);
    }

    /// <summary>
    /// Verifies that publishing an event calls all subscribed handlers.
    /// </summary>
    [Fact]
    public void PublishWithMultipleSubscribersCallsAllHandlers()
    {
        var publisher = new EventPublisher();
        var callCount = 0;

        publisher.SubscribeEvent<string>(_ => callCount++);
        publisher.SubscribeEvent<string>(_ => callCount++);

        publisher.PublishEvent("test");

        Assert.Equal(2, callCount);
    }

    /// <summary>
    /// Verifies that unsubscribing removes the handler from invocation list.
    /// </summary>
    [Fact]
    public void UnsubscribeRemovesTheHandler()
    {
        var publisher = new EventPublisher();
        var called = false;

        publisher.SubscribeEvent((Action<string>)Handler);
        publisher.UnsubscribeEvent((Action<string>)Handler);
        publisher.PublishEvent("test");

        Assert.False(called);
        return;

        void Handler(string value)
        {
            called = true;
        }
    }

    /// <summary>
    /// A test subscriber.
    /// </summary>
    private sealed class TestSubscriber
    {
        /// <summary>
        /// The action to invoke.
        /// </summary>
        private readonly Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSubscriber" /> class.
        /// </summary>
        /// <param name="publisher">The event publisher.</param>
        /// <param name="action">The action to invoke.</param>
        public TestSubscriber(IEventPublisher publisher, Action<string> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            publisher.SubscribeEvent<string>(this.Handler);
        }

        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="value">The event to handle.</param>
        private void Handler(string value)
        {
            this.action.Invoke(value);
        }
    }
}