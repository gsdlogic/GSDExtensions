// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeZoneService.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Blazor.Services;

using Microsoft.JSInterop;

/// <summary>
/// Provides services to get the local time zone.
/// </summary>
public sealed class TimeZoneService : IAsyncDisposable
{
    /// <summary>
    /// The module task.
    /// </summary>
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    /// <summary>
    /// The local timezone name.
    /// </summary>
    private string timezoneName;

    /// <summary>
    /// The local timezone offset.
    /// </summary>
    private TimeSpan? timezoneOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeZoneService" /> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    public TimeZoneService(IJSRuntime jsRuntime)
    {
        Task<IJSObjectReference> ValueFactory()
        {
            return jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/timezoneService.js").AsTask();
        }

        this.moduleTask = new Lazy<Task<IJSObjectReference>>(ValueFactory);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (this.moduleTask.IsValueCreated)
        {
            var module = await this.moduleTask.Value.ConfigureAwait(false);
            await module.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Converts the specified date and time to local time.
    /// </summary>
    /// <param name="dateTimeOffset">The date and time to convert.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result is the local date and time.</returns>
    public async ValueTask<DateTimeOffset> GetLocalDateTimeOffset(DateTimeOffset dateTimeOffset, CancellationToken cancellationToken = default)
    {
        var offset = this.timezoneOffset ?? await this.GetTimezoneOffset(cancellationToken).ConfigureAwait(false);
        return dateTimeOffset.ToOffset(offset);
    }

    /// <summary>
    /// Gets the local timezone name.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result is the local timezone name.</returns>
    public async ValueTask<string> GetTimezoneName(CancellationToken cancellationToken = default)
    {
        if (this.timezoneName != null)
        {
            return this.timezoneName;
        }

        var module = await this.moduleTask.Value.ConfigureAwait(false);
        this.timezoneName = await module.InvokeAsync<string>("getTimezoneName", cancellationToken).ConfigureAwait(false);

        return this.timezoneName;
    }

    /// <summary>
    /// Gets the local timezone offset.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result is the local timezone offset.</returns>
    public async ValueTask<TimeSpan> GetTimezoneOffset(CancellationToken cancellationToken = default)
    {
        if (this.timezoneOffset != null)
        {
            return this.timezoneOffset.Value;
        }

        var module = await this.moduleTask.Value.ConfigureAwait(false);
        var offsetInMinutes = await module.InvokeAsync<int>("getTimezoneOffset", cancellationToken).ConfigureAwait(false);
        this.timezoneOffset = TimeSpan.FromMinutes(-offsetInMinutes);

        return this.timezoneOffset.Value;
    }
}