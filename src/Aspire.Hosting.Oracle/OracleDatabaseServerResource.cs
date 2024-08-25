// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents an Oracle Database container.
/// </summary>
public class OracleDatabaseServerResource : ContainerResource, IResourceWithConnectionString
{
    internal const string PrimaryEndpointName = "tcp";

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleDatabaseServerResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="password">A parameter that contains the Oracle Database server password.</param>
    public OracleDatabaseServerResource(string name, ParameterResource password) : base(ThrowIfNullOrEmpty(name))
    {
        ThrowIfNullOrEmpty(password);

        PrimaryEndpoint = new(this, PrimaryEndpointName);
        PasswordParameter = password;
    }

    /// <summary>
    /// Gets the primary endpoint for the Oracle server.
    /// </summary>
    public EndpointReference PrimaryEndpoint { get; }

    /// <summary>
    /// Gets the parameter that contains the Oracle Database server password.
    /// </summary>
    public ParameterResource PasswordParameter { get; }

    /// <summary>
    /// Gets the connection string expression for the Oracle Database server.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"user id=system;password={PasswordParameter};data source={PrimaryEndpoint.Property(EndpointProperty.Host)}:{PrimaryEndpoint.Property(EndpointProperty.Port)}");

    private readonly Dictionary<string, string> _databases = new(StringComparers.ResourceName);

    /// <summary>
    /// A dictionary where the key is the resource name and the value is the database name.
    /// </summary>
    public IReadOnlyDictionary<string, string> Databases => _databases;

    internal void AddDatabase(string name, string databaseName)
    {
        _databases.TryAdd(name, databaseName);
    }

    private static T ThrowIfNullOrEmpty<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);

        if (argument is not string str)
        {
            return argument;
        }

        ArgumentException.ThrowIfNullOrEmpty(str, paramName);
        return argument;

    }
}
