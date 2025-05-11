// <copyright file="ICommandDispatcher.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a unified interface for sending commands.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Sends a command without expecting a response.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
}