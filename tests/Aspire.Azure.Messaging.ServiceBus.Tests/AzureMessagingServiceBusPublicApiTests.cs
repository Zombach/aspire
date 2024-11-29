// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Azure.Core.Extensions;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Azure.Messaging.ServiceBus.Tests;

public class AzureMessagingServiceBusPublicApiTests
{
    [Fact]
    public void AddAzureServiceBusClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "ServiceBus";

        var action = () => builder.AddAzureServiceBusClient(
            connectionName,
            default(Action<AzureMessagingServiceBusSettings>?),
            default(Action<IAzureClientBuilder<ServiceBusClient, ServiceBusClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddAzureServiceBusClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddAzureServiceBusClient(
            connectionName,
            default(Action<AzureMessagingServiceBusSettings>?),
            default(Action<IAzureClientBuilder<ServiceBusClient, ServiceBusClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedAzureServiceBusClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string name = "ServiceBus";

        var action = () => builder.AddKeyedAzureServiceBusClient(
            name,
            default(Action<AzureMessagingServiceBusSettings>?),
            default(Action<IAzureClientBuilder<ServiceBusClient, ServiceBusClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedAzureServiceBusClientShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var name = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedAzureServiceBusClient(
            name,
            default(Action<AzureMessagingServiceBusSettings>?),
            default(Action<IAzureClientBuilder<ServiceBusClient, ServiceBusClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}