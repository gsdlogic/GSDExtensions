// <copyright file="ICommandHandler.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a handler for processing a specific command type.
/// </summary>
/// <typeparam name="TCommand">The type of command this handler processes.</typeparam>
public interface ICommandHandler<in TCommand>
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}