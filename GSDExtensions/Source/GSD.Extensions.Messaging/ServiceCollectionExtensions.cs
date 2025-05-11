// <copyright file="ServiceCollectionExtensions.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection" /> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds command processing to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection so that additional calls may be chained.</returns>
    public static IServiceCollection AddCommandDispatching(this IServiceCollection services)
    {
        services.TryAddSingleton<IServiceProviderStack, ServiceProviderStack>();
        return services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
    }

    /// <summary>
    /// Adds event publishing to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection so that additional calls may be chained.</returns>
    public static IServiceCollection AddEventPublishing(this IServiceCollection services)
    {
        return services.AddSingleton<IEventPublisher, EventPublisher>();
    }

    /// <summary>
    /// Adds event publishing to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection so that additional calls may be chained.</returns>
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddCommandDispatching();
        services.AddQueryDispatching();
        services.AddEventPublishing();

        return services.AddSingleton<IMessageBus>(context => new MessageBus(
            context.GetRequiredService<ICommandDispatcher>(),
            context.GetRequiredService<IQueryDispatcher>(),
            context.GetRequiredService<IEventPublisher>()));
    }

    /// <summary>
    /// Adds event publishing to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection so that additional calls may be chained.</returns>
    public static IServiceCollection AddQueryDispatching(this IServiceCollection services)
    {
        services.TryAddSingleton<IServiceProviderStack, ServiceProviderStack>();
        return services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
    }
}