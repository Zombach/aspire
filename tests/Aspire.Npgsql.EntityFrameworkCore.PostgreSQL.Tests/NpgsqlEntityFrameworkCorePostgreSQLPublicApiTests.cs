// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Npgsql.EntityFrameworkCore.PostgreSQL.Tests;

public class NpgsqlEntityFrameworkCorePostgreSQLPublicApiTests
{
    [Fact]
    public void AddNpgsqlDbContextShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "npgsql";

        var action = () => builder.AddNpgsqlDbContext<DbContext>(
            connectionName,
            default(Action<NpgsqlEntityFrameworkCorePostgreSQLSettings>?),
            default(Action<DbContextOptionsBuilder>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddNpgsqlDbContextShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddNpgsqlDbContext<DbContext>(
            connectionName,
            default(Action<NpgsqlEntityFrameworkCorePostgreSQLSettings>?),
            default(Action<DbContextOptionsBuilder>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void EnrichNpgsqlDbContextShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;

        var action = () => builder.EnrichNpgsqlDbContext<DbContext>(default(Action<NpgsqlEntityFrameworkCorePostgreSQLSettings>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }
}