// <copyright file="BurstThrottler.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Messaging;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A leading-edge throttler with a trailing-edge debounce, executing
/// immediately and once more after a quiet period if re-triggered.
/// </summary>
/// ReSharper disable AsyncVoidMethod
public class BurstThrottler
{
    /// <summary>
    /// The user-defined action to execute when <see cref="Invoke" /> is called.
    /// </summary>
    private readonly Action action;

    /// <summary>
    /// The user-defined error handler for exceptions thrown by the action.
    /// </summary>
    private readonly Action<Exception> errorHandler;

    /// <summary>
    /// The minimum interval between successive action executions.
    /// </summary>
    private readonly TimeSpan interval;

    /// <summary>
    /// Tracks the number of calls to <see cref="Invoke" /> during an interval.
    /// </summary>
    private int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="BurstThrottler" /> class.
    /// </summary>
    /// <param name="action">The user-defined action to execute when <see cref="Invoke" /> is called.</param>
    /// <param name="errorHandler">The user-defined error handler for exceptions thrown by the action.</param>
    /// <param name="interval">The minimum interval between successive action executions. Defaults to 300ms if not provided.</param>
    public BurstThrottler(Action action, Action<Exception> errorHandler = null, TimeSpan interval = default)
    {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
        this.errorHandler = errorHandler;
        this.interval = interval <= TimeSpan.Zero ? TimeSpan.FromMilliseconds(300) : interval;
    }

    /// <summary>
    /// Executes the action immediately on the first call, then suppresses further calls
    /// during the defined interval. If additional calls are made during that interval,
    /// the action is invoked again after interval expires.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "General exception handler, specific exception is unknown.")]
    public async void Invoke()
    {
        if (Interlocked.Increment(ref this.count) != 1)
        {
            return;
        }

        while (true)
        {
            try
            {
                this.action.Invoke();
            }
            catch (Exception ex)
            {
                this.errorHandler?.Invoke(ex);
            }

            await Task.Delay(this.interval).ConfigureAwait(false);

            if (Interlocked.CompareExchange(ref this.count, 0, 1) == 1)
            {
                return;
            }

            Interlocked.Exchange(ref this.count, 1);
        }
    }
}