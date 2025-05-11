// <copyright file="IServiceProviderStack.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides a stack-based mechanism for managing scoped <see cref="IServiceProvider" /> instances.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Not a collection, but behaves like a stack internally to manage DI scopes.")]
public interface IServiceProviderStack
{
    /// <summary>
    /// Gets the currently active <see cref="IServiceProvider" />.
    /// If no scope has been pushed, returns the root-level service provider.
    /// </summary>
    IServiceProvider Current { get; }

    /// <summary>
    /// Pops the current <see cref="IServiceProvider" /> off the stack,
    /// restoring the previous scope or falling back to the root provider.
    /// </summary>
    void Pop();

    /// <summary>
    /// Pushes a new scoped <see cref="IServiceProvider" /> onto the stack,
    /// making it the new <see cref="Current" /> provider.
    /// </summary>
    /// <param name="serviceProvider">The scoped service provider to set as current.</param>
    void Push(IServiceProvider serviceProvider);
}