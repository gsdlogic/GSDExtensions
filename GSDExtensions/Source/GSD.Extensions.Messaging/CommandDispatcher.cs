// <copyright file="CommandDispatcher.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Default implementation of <see cref="ICommandDispatcher" /> that resolves command handlers from the active service scope.
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    /// <summary>
    /// The scoped service provider stack used to resolve command handlers.
    /// </summary>
    private readonly IServiceProviderStack serviceProviderStack;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDispatcher" /> class.
    /// </summary>
    /// <param name="serviceProviderStack">The scoped service provider stack used to resolve command handlers.</param>
    public CommandDispatcher(IServiceProviderStack serviceProviderStack)
    {
        this.serviceProviderStack = serviceProviderStack ?? throw new ArgumentNullException(nameof(serviceProviderStack));
    }

    /// <summary>
    /// Sends a command without expecting a response.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var handlerType = typeof(ICommandHandler<TCommand>);
        var provider = this.serviceProviderStack.Current;

        if (provider.GetService(handlerType) is not ICommandHandler<TCommand> handler)
        {
            throw new InvalidOperationException($"No command handler registered for type {handlerType.FullName}.");
        }

        await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }
}