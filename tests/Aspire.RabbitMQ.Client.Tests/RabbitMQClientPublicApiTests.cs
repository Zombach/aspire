// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Xunit;

namespace Aspire.RabbitMQ.Client.Tests;

public class RabbitMQClientPublicApiTests
{
    [Fact]
    public void AddRabbitMQClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "rabbitmq";

        var action = () => builder.AddRabbitMQClient(
            connectionName,
            default(Action<RabbitMQClientSettings>?),
            default(Action<ConnectionFactory>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddRabbitMQClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddRabbitMQClient(
            connectionName,
            default(Action<RabbitMQClientSettings>?),
            default(Action<ConnectionFactory>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedRabbitMQClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string name = "rabbitmq";

        var action = () => builder.AddKeyedRabbitMQClient(
            name,
            default(Action<RabbitMQClientSettings>?),
            default(Action<ConnectionFactory>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedRabbitMQClientShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var name = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedRabbitMQClient(
            name,
            default(Action<RabbitMQClientSettings>?),
            default(Action<ConnectionFactory>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}