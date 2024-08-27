// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Aspire.Hosting.ConsoleLogs;
using Microsoft.Extensions.Logging;

namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A service that provides loggers for resources to write to.
/// </summary>
public class ResourceLoggerService
{
    // Internal for testing.
    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    private readonly ConcurrentDictionary<string, ResourceLoggerState> _loggers = new();
    private Action<(string, ResourceLoggerState)>? _loggerAdded;
    private event Action<(string, ResourceLoggerState)> LoggerAdded
    {
        add
        {
            _loggerAdded += value;

            foreach (var logger in _loggers)
            {
                value((logger.Key, logger.Value));
            }
        }
        remove
        {
            _loggerAdded -= value;
        }
    }

    /// <summary>
    /// Gets the logger for the resource to write to.
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <returns>An <see cref="ILogger"/> which represents the resource.</returns>
    public ILogger GetLogger(IResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        return GetResourceLoggerState(resource.Name).Logger;
    }

    /// <summary>
    /// Gets the logger for the resource to write to.
    /// </summary>
    /// <param name="resourceName">The name of the resource from the Aspire application model.</param>
    /// <returns>An <see cref="ILogger"/> which represents the named resource.</returns>
    public ILogger GetLogger(string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        return GetResourceLoggerState(resourceName).Logger;
    }

    /// <summary>
    /// The internal logger is used when adding logs from resource's stream logs.
    /// It allows the parsed date from text to be used as the log line date.
    /// </summary>
    internal Action<DateTime?, string, bool> GetInternalLogger(string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        return GetResourceLoggerState(resourceName).AddLog;
    }

    /// <summary>
    /// Watch for changes to the log stream for a resource.
    /// </summary>
    /// <param name="resource">The resource to watch for logs.</param>
    /// <returns>An async enumerable that returns the logs as they are written.</returns>
    public IAsyncEnumerable<IReadOnlyList<LogLine>> WatchAsync(IResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        return WatchAsync(resource.Name);
    }

    /// <summary>
    /// Watch for changes to the log stream for a resource.
    /// </summary>
    /// <param name="resourceName">The resource name</param>
    /// <returns>An async enumerable that returns the logs as they are written.</returns>
    public IAsyncEnumerable<IReadOnlyList<LogLine>> WatchAsync(string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        return GetResourceLoggerState(resourceName).WatchAsync();
    }

    /// <summary>
    /// Watch for subscribers to the log stream for a resource.
    /// </summary>
    /// <returns>
    /// An async enumerable that returns when the first subscriber is added to a log,
    /// or when the last subscriber is removed.
    /// </returns>
    public async IAsyncEnumerable<LogSubscriber> WatchAnySubscribersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<LogSubscriber>();

        void OnLoggerAdded((string Name, ResourceLoggerState State) loggerItem)
        {
            var (name, state) = loggerItem;

            state.OnSubscribersChanged += (hasSubscribers) =>
            {
                channel.Writer.TryWrite(new(name, hasSubscribers));
            };
        }

        LoggerAdded += OnLoggerAdded;

        try
        {
            await foreach (var entry in channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return entry;
            }
        }
        finally
        {
            LoggerAdded -= OnLoggerAdded;
        }
    }

    /// <summary>
    /// Completes the log stream for the resource.
    /// </summary>
    /// <param name="resource">The <see cref="IResource"/>.</param>
    public void Complete(IResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        if (_loggers.TryGetValue(resource.Name, out var logger))
        {
            logger.Complete();
        }
    }

    /// <summary>
    /// Completes the log stream for the resource.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    public void Complete(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (_loggers.TryGetValue(name, out var logger))
        {
            logger.Complete();
        }
    }

    /// <summary>
    /// Clears the log stream's backlog for the resource.
    /// </summary>
    public void ClearBacklog(string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        if (_loggers.TryGetValue(resourceName, out var logger))
        {
            logger.ClearBacklog();
        }
    }

    // Internal for testing.
    internal ResourceLoggerState GetResourceLoggerState(string resourceName) =>
        _loggers.GetOrAdd(resourceName, (name, context) =>
        {
            var state = new ResourceLoggerState(TimeProvider);
            context._loggerAdded?.Invoke((name, state));
            return state;
        },
        this);

    /// <summary>
    /// A logger for the resource to write to.
    /// </summary>
    internal sealed class ResourceLoggerState
    {
        private readonly ResourceLogger _logger;
        private readonly CancellationTokenSource _logStreamCts = new();
        private readonly object _lock = new();

        private readonly LogEntries _backlog = new(10000) { BaseLineNumber = 0 };
        private readonly TimeProvider _timeProvider;

        /// <summary>
        /// Creates a new <see cref="ResourceLoggerState"/>.
        /// </summary>
        public ResourceLoggerState(TimeProvider timeProvider)
        {
            _logger = new ResourceLogger(this);
            _timeProvider = timeProvider;
        }

        private Action<bool>? _onSubscribersChanged;
        public event Action<bool> OnSubscribersChanged
        {
            add
            {
                _onSubscribersChanged += value;

                var hasSubscribers = false;

                lock (_lock)
                {
                    if (_onNewLog is not null) // we have subscribers
                    {
                        hasSubscribers = true;
                    }
                }

                if (hasSubscribers)
                {
                    value(hasSubscribers);
                }
            }
            remove
            {
                _onSubscribersChanged -= value;
            }
        }

        /// <summary>
        /// Watch for changes to the log stream for a resource.
        /// </summary>
        /// <returns>The log stream for the resource.</returns>
        public async IAsyncEnumerable<IReadOnlyList<LogLine>> WatchAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Line number always restarts from 1 when watching logs.
            // Note that this will need to be improved if the log source (DCP) is changed to return a maximum number of lines.
            var lineNumber = 1;
            var channel = Channel.CreateUnbounded<LogEntry>();

            using var _ = _logStreamCts.Token.Register(() => channel.Writer.TryComplete());

            // No need to lock in the log method because TryWrite/TryComplete are already threadsafe.
            void Log(LogEntry log) => channel.Writer.TryWrite(log);

            LogEntry[] backlogSnapshot;
            lock (_lock)
            {
                // Get back
                backlogSnapshot = GetBacklogSnapshot();
                OnNewLog += Log;
            }

            if (backlogSnapshot.Length > 0)
            {
                yield return CreateLogLines(ref lineNumber, backlogSnapshot);
            }

            try
            {
                await foreach (var entry in channel.GetBatchesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    yield return CreateLogLines(ref lineNumber, entry);
                }
            }
            finally
            {
                lock (_lock)
                {
                    OnNewLog -= Log;
                    channel.Writer.TryComplete();
                }
            }

            static LogLine[] CreateLogLines(ref int lineNumber, IReadOnlyList<LogEntry> entries)
            {
                var logs = new LogLine[entries.Count];
                for (var i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var content = entry.Content ?? string.Empty;
                    if (entry.Timestamp != null)
                    {
                        content = entry.Timestamp.Value.ToString(KnownFormats.ConsoleLogsTimestampFormat, CultureInfo.InvariantCulture) + " " + content;
                    }

                    logs[i] = new LogLine(lineNumber, content, entry.Type == LogEntryType.Error);
                    lineNumber++;
                }

                return logs;
            }
        }

        // This provides the fan out to multiple subscribers.
        private Action<LogEntry>? _onNewLog;
        private event Action<LogEntry> OnNewLog
        {
            add
            {
                Debug.Assert(Monitor.IsEntered(_lock));

                // When this is the first subscriber, raise event so WatchAnySubscribersAsync publishes an update.
                // Is this the first subscriber?
                var raiseSubscribersChanged = _onNewLog is null;

                _onNewLog += value;

                if (raiseSubscribersChanged)
                {
                    _onSubscribersChanged?.Invoke(true);
                }
            }
            remove
            {
                Debug.Assert(Monitor.IsEntered(_lock));

                _onNewLog -= value;

                // When there are no more subscribers, raise event so WatchAnySubscribersAsync publishes an update.
                // Is this the last subscriber?
                var raiseSubscribersChanged = _onNewLog is null;
                if (raiseSubscribersChanged)
                {
                    // Clear backlog immediately.
                    // Avoids a race between message being subscription changed notification eventually clearing the
                    // logs and someone else watching logs and getting the backlog + complete replay off all logs.
                    ClearBacklog();

                    _onSubscribersChanged?.Invoke(false);
                }
            }
        }

        /// <summary>
        /// The logger for the resource to write to. This will write updates to the live log stream for this resource.
        /// </summary>
        public ILogger Logger => _logger;

        /// <summary>
        /// Close the log stream for the resource. Future subscribers will not receive any updates and will complete immediately.
        /// </summary>
        public void Complete()
        {
            // REVIEW: Do we clean up the backlog?
            _logStreamCts.Cancel();
        }

        public void ClearBacklog()
        {
            lock (_lock)
            {
                _backlog.Clear();
                _backlog.BaseLineNumber = 0;
            }
        }

        internal LogEntry[] GetBacklogSnapshot()
        {
            lock (_lock)
            {
                return [.. _backlog.GetEntries()];
            }
        }

        public void AddLog(DateTime? timestamp, string logMessage, bool isErrorMessage)
        {
            var logEntry = new LogEntry { Timestamp = timestamp, Content = logMessage, Type = isErrorMessage ? LogEntryType.Error : LogEntryType.Default };
            lock (_lock)
            {
                _backlog.InsertSorted(logEntry);
            }

            _onNewLog?.Invoke(logEntry);
        }

        private sealed class ResourceLogger(ResourceLoggerState loggerState) : ILogger
        {
            IDisposable? ILogger.BeginScope<TState>(TState state) => null;

            bool ILogger.IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (loggerState._logStreamCts.IsCancellationRequested)
                {
                    // Noop if logging after completing the stream
                    return;
                }

                var logTime = loggerState._timeProvider.GetUtcNow().UtcDateTime;

                var logMessage = formatter(state, exception) + (exception is null ? "" : $"\n{exception}");
                var isErrorMessage = logLevel >= LogLevel.Error;

                loggerState.AddLog(logTime, logMessage, isErrorMessage);
            }
        }
    }
}

/// <summary>
/// Represents a log subscriber for a resource.
/// </summary>
/// <param name="Name">The the resource name.</param>
/// <param name="AnySubscribers">Determines if there are any subscribers.</param>
public readonly record struct LogSubscriber(string Name, bool AnySubscribers);
