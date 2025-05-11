// <copyright file="CommandDispatcherTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging.UnitTests;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides unit tests for the <see cref="CommandDispatcher" /> class.
/// </summary>
public class CommandDispatcherTests
{
    /// <summary>
    /// The service provider options.
    /// </summary>
    private static readonly ServiceProviderOptions ServiceProviderOptions = new()
    {
        ValidateScopes = true,
        ValidateOnBuild = true,
    };

    /// <summary>
    /// Ensures that a scoped command can not be resolved after an active scope is disposed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task CannotResolveScopedCommandWithDisposedActiveScope()
    {
        var services = new ServiceCollection();
        services.AddCommandDispatching();
        services.AddScoped<ICommandHandler<TestCommand>, TestCommandHandler>();
        services.AddSingleton<CommandCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        using (var scope = provider.CreateScope())
        {
            var stack = scope.ServiceProvider.GetRequiredService<IServiceProviderStack>();
            stack.Push(scope.ServiceProvider);
            stack.Pop();
        }

        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await dispatcher.SendCommandAsync(command).ConfigureAwait(true));

        var counter = provider.GetRequiredService<CommandCounter>();
        Assert.Equal(0, counter.Value);
    }

    /// <summary>
    /// Ensures that a scoped command can not be resolved with no active scope.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task CannotResolveScopedCommandWithNoActiveScope()
    {
        var services = new ServiceCollection();
        services.AddCommandDispatching();
        services.AddScoped<ICommandHandler<TestCommand>, TestCommandHandler>();
        services.AddSingleton<CommandCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await dispatcher.SendCommandAsync(command).ConfigureAwait(true));

        var counter = provider.GetRequiredService<CommandCounter>();
        Assert.Equal(0, counter.Value);
    }

    /// <summary>
    /// Ensures that a scoped command can be resolved with an active scope.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task ResolveScopedCommandWithActiveScope()
    {
        var services = new ServiceCollection();
        services.AddCommandDispatching();
        services.AddScoped<ICommandHandler<TestCommand>, TestCommandHandler>();
        services.AddSingleton<CommandCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        using var scope = provider.CreateScope();
        var stack = scope.ServiceProvider.GetRequiredService<IServiceProviderStack>();
        stack.Push(scope.ServiceProvider);

        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand();
        await dispatcher.SendCommandAsync(command);

        var counter = provider.GetRequiredService<CommandCounter>();
        Assert.Equal(1, counter.Value);
    }

    /// <summary>
    /// Ensures that a singleton command can be resolved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task ResolvesSingletonCommand()
    {
        var services = new ServiceCollection();
        services.AddCommandDispatching();
        services.AddSingleton<ICommandHandler<TestCommand>, TestCommandHandler>();
        services.AddSingleton<CommandCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand();
        await dispatcher.SendCommandAsync(command);

        var counter = provider.GetRequiredService<CommandCounter>();
        Assert.Equal(1, counter.Value);
    }

    /// <summary>
    /// Provides a counter for the test command.
    /// </summary>
    private sealed class CommandCounter
    {
        /// <summary>
        /// Gets or sets the value of the counter.
        /// </summary>
        public int Value { get; set; }
    }

    /// <summary>
    /// Provides a command for testing.
    /// </summary>
    private sealed class TestCommand
    {
    }

    /// <summary>
    /// Provides a handler for the test command.
    /// </summary>
    private sealed class TestCommandHandler : ICommandHandler<TestCommand>
    {
        /// <summary>
        /// The command counter.
        /// </summary>
        private readonly CommandCounter counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCommandHandler" /> class.
        /// </summary>
        /// <param name="counter">The command counter.</param>
        public TestCommandHandler(CommandCounter counter)
        {
            this.counter = counter ?? throw new ArgumentNullException(nameof(counter));
        }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
        {
            this.counter.Value++;
            return Task.CompletedTask;
        }
    }
}