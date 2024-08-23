// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Azure.Core.Extensions;
using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Azure.Messaging.WebPubSub.Tests;

public class AzureMessagingWebPubSubPublicApiTests
{
    [Fact]
    public void AddAzureWebPubSubServiceClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "wps";

        var action = () => builder.AddAzureWebPubSubServiceClient(
            connectionName,
            default(Action<AzureMessagingWebPubSubSettings>?),
            default(Action<IAzureClientBuilder<WebPubSubServiceClient, WebPubSubServiceClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddAzureWebPubSubServiceClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddAzureWebPubSubServiceClient(
            connectionName,
            default(Action<AzureMessagingWebPubSubSettings>?),
            default(Action<IAzureClientBuilder<WebPubSubServiceClient, WebPubSubServiceClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedAzureWebPubSubServiceClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "wps";
        const string serviceKey = "wps";

        var action = () => builder.AddKeyedAzureWebPubSubServiceClient(
            connectionName,
            serviceKey,
            default(Action<AzureMessagingWebPubSubSettings>?),
            default(Action<IAzureClientBuilder<WebPubSubServiceClient, WebPubSubServiceClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedAzureWebPubSubServiceClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;
        const string serviceKey = "wps";

        var action = () => builder.AddKeyedAzureWebPubSubServiceClient(
            connectionName,
            serviceKey,
            default(Action<AzureMessagingWebPubSubSettings>?),
            default(Action<IAzureClientBuilder<WebPubSubServiceClient, WebPubSubServiceClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedAzureWebPubSubServiceClientShouldThrowWhenServiceKeyIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        const string connectionName = "wps";
        var serviceKey = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedAzureWebPubSubServiceClient(
            connectionName,
            serviceKey,
            default(Action<AzureMessagingWebPubSubSettings>?),
            default(Action<IAzureClientBuilder<WebPubSubServiceClient, WebPubSubServiceClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(serviceKey), exception.ParamName);
    }
}
