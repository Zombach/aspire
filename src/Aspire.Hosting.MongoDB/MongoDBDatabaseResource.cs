// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents a MongoDB database. This is a child resource of a <see cref="MongoDBServerResource"/>.
/// </summary>
public class MongoDBDatabaseResource : Resource, IResourceWithParent<MongoDBServerResource>, IResourceWithConnectionString
{
    /// <summary>
    /// A resource that represents a MongoDB database. This is a child resource of a <see cref="MongoDBServerResource"/>.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="parent">The MongoDB server resource associated with this database.</param>
    public MongoDBDatabaseResource(string name, string databaseName, MongoDBServerResource parent) : base(ThrowIfNullOrEmpty(name))
    {
        ThrowIfNullOrEmpty(databaseName);
        ArgumentNullException.ThrowIfNull(parent);

        Parent = parent;
        DatabaseName = databaseName;
    }

    /// <summary>
    /// Gets the connection string expression for the MongoDB database.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression => Parent.BuildConnectionString(DatabaseName);

    /// <summary>
    /// Gets the parent MongoDB container resource.
    /// </summary>
    public MongoDBServerResource Parent { get; }

    /// <summary>
    /// Gets the database name.
    /// </summary>
    public string DatabaseName { get; }

    private static string ThrowIfNullOrEmpty(
        [NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }
}
