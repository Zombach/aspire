// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Azure.Core.Extensions;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Azure.Storage.Blobs.Tests;

public class AzureStorageBlobsPublicApiTests
{
    [Fact]
    public void AddAzureBlobClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "blobs";

        var action = () => builder.AddAzureBlobClient(
            connectionName,
            default(Action<AzureStorageBlobsSettings>?),
            default(Action<IAzureClientBuilder<BlobServiceClient, BlobClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddAzureBlobClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddAzureBlobClient(
            connectionName,
            default(Action<AzureStorageBlobsSettings>?),
            default(Action<IAzureClientBuilder<BlobServiceClient, BlobClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedAzureBlobClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string name = "blobs";

        var action = () => builder.AddKeyedAzureBlobClient(
            name,
            default(Action<AzureStorageBlobsSettings>?),
            default(Action<IAzureClientBuilder<BlobServiceClient, BlobClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedAzureBlobClientShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var name = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedAzureBlobClient(
            name,
            default(Action<AzureStorageBlobsSettings>?),
            default(Action<IAzureClientBuilder<BlobServiceClient, BlobClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}
