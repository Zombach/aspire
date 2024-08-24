// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Pomelo.EntityFrameworkCore.MySql.Tests;

public class PomeloEntityFrameworkCoreMySqlPublicApiTests
{
    [Fact]
    public void AddMySqlDbContextShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "mysql";

        var action = () => builder.AddMySqlDbContext<DbContext>(
            connectionName,
            default(Action<PomeloEntityFrameworkCoreMySqlSettings>?),
            default(Action<DbContextOptionsBuilder>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddMySqlDbContextShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddMySqlDbContext<DbContext>(
            connectionName,
            default(Action<PomeloEntityFrameworkCoreMySqlSettings>?),
            default(Action<DbContextOptionsBuilder>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void EnrichMySqlDbContextShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;

        var action = () => builder.EnrichMySqlDbContext<DbContext>(default(Action<PomeloEntityFrameworkCoreMySqlSettings>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }
}
