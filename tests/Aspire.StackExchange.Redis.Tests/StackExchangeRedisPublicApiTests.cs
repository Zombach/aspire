// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Xunit;

namespace Aspire.StackExchange.Redis.Tests;

public class StackExchangeRedisPublicApiTests
{
    [Fact]
    public void AddRedisClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "redis";

        var action = () => builder.AddRedisClient(
            connectionName,
            default(Action<StackExchangeRedisSettings>?),
            default(Action<ConfigurationOptions>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddRedisClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddRedisClient(
            connectionName,
            default(Action<StackExchangeRedisSettings>?),
            default(Action<ConfigurationOptions>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedRedisClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string name = "redis";

        var action = () => builder.AddKeyedRedisClient(
            name,
            default(Action<StackExchangeRedisSettings>?),
            default(Action<ConfigurationOptions>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedRedisClientShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var name = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedRedisClient(
            name,
            default(Action<StackExchangeRedisSettings>?),
            default(Action<ConfigurationOptions>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}
