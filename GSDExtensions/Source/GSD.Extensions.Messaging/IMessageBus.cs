// <copyright file="IMessageBus.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

/// <summary>
/// Defines a unified interface for sending commands, queries, and publishing events.
/// </summary>
public interface IMessageBus : ICommandDispatcher, IQueryDispatcher, IEventPublisher
{
}