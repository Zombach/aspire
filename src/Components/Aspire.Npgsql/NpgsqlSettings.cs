// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Npgsql;

/// <summary>
/// Provides the client configuration settings for connecting to a PostgreSQL database using Npgsql.
/// </summary>
public sealed class NpgsqlSettings
{
    /// <summary>
    /// The connection string of the PostgreSQL database to connect to.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the database health check is enabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true"/>.
    /// </value>
    [Obsolete($"This property is obsolete and will be removed in a future version. Use {nameof(DisableHealthChecks)} instead.")]
    public bool HealthChecks { get; set; } = true;

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the database health check is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableHealthChecks { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is enabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true" />.
    /// </value>
    [Obsolete($"This property is obsolete and will be removed in a future version. Use {nameof(DisableTracing)} instead.")]
    public bool Tracing { get; set; } = true;

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableTracing { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry metrics are enabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true" />.
    /// </value>
    [Obsolete($"This property is obsolete and will be removed in a future version. Use {nameof(DisableMetrics)} instead.")]
    public bool Metrics { get; set; } = true;

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry metrics are disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableMetrics { get; set; }
}
