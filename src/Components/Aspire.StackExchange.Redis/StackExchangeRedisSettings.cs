// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.StackExchange.Redis;

/// <summary>
/// Provides the client configuration settings for connecting to a Redis server.
/// </summary>
public sealed class StackExchangeRedisSettings
{
    /// <summary>
    /// Gets or sets the comma-delimited configuration string used to connect to the Redis server.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the Redis health check is enabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true"/>.
    /// </value>
    public bool HealthChecksEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is enabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true"/>.
    /// </value>
    public bool TracingEnabled { get; set; } = true;
}
