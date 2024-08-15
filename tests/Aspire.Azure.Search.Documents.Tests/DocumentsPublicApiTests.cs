// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Azure.Core.Extensions;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Azure.Search.Documents.Tests;

public class DocumentsPublicApiTests
{
    [Fact]
    public void AddAzureSearchClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "search";

        var action = () => builder.AddAzureSearchClient(
            connectionName,
            default(Action<AzureSearchSettings>?),
            default(Action<IAzureClientBuilder<SearchIndexClient, SearchClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddAzureSearchClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddAzureSearchClient(connectionName);

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedAzureSearchClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string name = "search";

        var action = () => builder.AddKeyedAzureSearchClient(
            name,
            default(Action<AzureSearchSettings>?),
            default(Action<IAzureClientBuilder<SearchIndexClient, SearchClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedAzureSearchClientShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        var name = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedAzureSearchClient(
            name,
            default(Action<AzureSearchSettings>?),
            default(Action<IAzureClientBuilder<SearchIndexClient, SearchClientOptions>>?));

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}
