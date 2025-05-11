// <copyright file="QueryDispatcherTests.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging.UnitTests;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides unit tests for the <see cref="QueryDispatcher" /> class.
/// </summary>
public class QueryDispatcherTests
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
    /// Ensures that a scoped query can not be resolved after an active scope is disposed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task CannotResolveScopedQueryWithDisposedActiveScope()
    {
        var services = new ServiceCollection();
        services.AddQueryDispatching();
        services.AddScoped<IQueryHandler<TestQuery, TestResult>, TestQueryHandler>();
        services.AddSingleton<QueryCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        using (var scope = provider.CreateScope())
        {
            var stack = scope.ServiceProvider.GetRequiredService<IServiceProviderStack>();
            stack.Push(scope.ServiceProvider);
            stack.Pop();
        }

        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await dispatcher.SendQueryAsync<TestQuery, TestResult>(query).ConfigureAwait(true));

        var counter = provider.GetRequiredService<QueryCounter>();
        Assert.Equal(0, counter.Value);
    }

    /// <summary>
    /// Ensures that a scoped query can not be resolved with no active scope.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task CannotResolveScopedQueryWithNoActiveScope()
    {
        var services = new ServiceCollection();
        services.AddQueryDispatching();
        services.AddScoped<IQueryHandler<TestQuery, TestResult>, TestQueryHandler>();
        services.AddSingleton<QueryCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await dispatcher.SendQueryAsync<TestQuery, TestResult>(query).ConfigureAwait(true));

        var counter = provider.GetRequiredService<QueryCounter>();
        Assert.Equal(0, counter.Value);
    }

    /// <summary>
    /// Ensures that a scoped query can be resolved with an active scope.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task ResolveScopedQueryWithActiveScope()
    {
        var services = new ServiceCollection();
        services.AddQueryDispatching();
        services.AddScoped<IQueryHandler<TestQuery, TestResult>, TestQueryHandler>();
        services.AddSingleton<QueryCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        using var scope = provider.CreateScope();
        var stack = scope.ServiceProvider.GetRequiredService<IServiceProviderStack>();
        stack.Push(scope.ServiceProvider);

        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery();
        var result = await dispatcher.SendQueryAsync<TestQuery, TestResult>(query);
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);

        var counter = provider.GetRequiredService<QueryCounter>();
        Assert.Equal(1, counter.Value);
    }

    /// <summary>
    /// Ensures that a singleton query can be resolved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task ResolvesSingletonQuery()
    {
        var services = new ServiceCollection();
        services.AddQueryDispatching();
        services.AddSingleton<IQueryHandler<TestQuery, TestResult>, TestQueryHandler>();
        services.AddSingleton<QueryCounter>();

        var provider = services.BuildServiceProvider(ServiceProviderOptions);

        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery();
        var result = await dispatcher.SendQueryAsync<TestQuery, TestResult>(query);
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);

        var counter = provider.GetRequiredService<QueryCounter>();
        Assert.Equal(1, counter.Value);
    }

    /// <summary>
    /// Provides a counter for the test query.
    /// </summary>
    private sealed class QueryCounter
    {
        /// <summary>
        /// Gets or sets the value of the counter.
        /// </summary>
        public int Value { get; set; }
    }

    /// <summary>
    /// Provides a query for testing.
    /// </summary>
    private sealed class TestQuery
    {
    }

    /// <summary>
    /// Provides a handler for the test query.
    /// </summary>
    private sealed class TestQueryHandler : IQueryHandler<TestQuery, TestResult>
    {
        /// <summary>
        /// The query counter.
        /// </summary>
        private readonly QueryCounter counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestQueryHandler" /> class.
        /// </summary>
        /// <param name="counter">The query counter.</param>
        public TestQueryHandler(QueryCounter counter)
        {
            this.counter = counter ?? throw new ArgumentNullException(nameof(counter));
        }

        /// <summary>
        /// Handles the query and returns the result.
        /// </summary>
        /// <param name="query">The query to process.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the operation, with the result as <see cref="TestResult" />.</returns>
        public Task<TestResult> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
        {
            this.counter.Value++;

            var result = new TestResult
            {
                Count = this.counter.Value,
            };

            return Task.FromResult(result);
        }
    }

    /// <summary>
    /// Provides a result for testing.
    /// </summary>
    private sealed class TestResult
    {
        /// <summary>
        /// Gets the value from the counter.
        /// </summary>
        public int Count { get; init; }
    }
}