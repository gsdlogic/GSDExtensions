// <copyright file="ServiceProviderStack.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides a stack-based mechanism for managing scoped <see cref="IServiceProvider" /> instances.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Not a collection, but behaves like a stack internally to manage DI scopes.")]
public class ServiceProviderStack : IServiceProviderStack
{
    /// <summary>
    /// The stack of service providers.
    /// </summary>
    private readonly Stack<IServiceProvider> stack = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceProviderStack" /> class.
    /// </summary>
    /// <param name="rootProvider">The root service provider.</param>
    public ServiceProviderStack(IServiceProvider rootProvider)
    {
        this.Current = rootProvider;
    }

    /// <summary>
    /// Gets the currently active <see cref="IServiceProvider" />.
    /// If no scope has been pushed, returns the root-level service provider.
    /// </summary>
    public IServiceProvider Current { get; private set; }

    /// <summary>
    /// Pops the current <see cref="IServiceProvider" /> off the stack,
    /// restoring the previous scope or falling back to the root provider.
    /// </summary>
    public void Pop()
    {
        if (this.stack.Count == 0)
        {
            throw new InvalidOperationException("Cannot pop from an empty service provider stack.");
        }

        this.Current = this.stack.Pop();
    }

    /// <summary>
    /// Pushes a new scoped <see cref="IServiceProvider" /> onto the stack,
    /// making it the new <see cref="IServiceProviderStack.Current" /> provider.
    /// </summary>
    /// <param name="serviceProvider">The scoped service provider to set as current.</param>
    public void Push(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        this.stack.Push(this.Current);
        this.Current = serviceProvider;
    }
}