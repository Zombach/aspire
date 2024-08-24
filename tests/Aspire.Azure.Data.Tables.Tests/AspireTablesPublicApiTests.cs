// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Azure.Core.Extensions;
using Azure.Data.Tables;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Aspire.Azure.Data.Tables.Tests;

public class AspireTablesPublicApiTests
{
    [Fact]
    public void AddAzureTableClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string connectionName = "tables";

        var action = () => builder.AddAzureTableClient(
            connectionName,
            default(Action<AzureDataTablesSettings>?),
            default(Action<IAzureClientBuilder<TableServiceClient, TableClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddAzureTableClientShouldThrowWhenConnectionNameIsNullOrEmpty(bool isNull)
    {
        var builder = new HostApplicationBuilder();
        var connectionName = isNull ? null! : string.Empty;

        var action = () => builder.AddAzureTableClient(
            connectionName,
            default(Action<AzureDataTablesSettings>?),
            default(Action<IAzureClientBuilder<TableServiceClient, TableClientOptions>>?));

        var exception = isNull ? Assert.Throws<ArgumentNullException>(action) : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(connectionName), exception.ParamName);
    }

    [Fact]
    public void AddKeyedAzureTableClientShouldThrowWhenBuilderIsNull()
    {
        IHostApplicationBuilder builder = null!;
        const string name = "tables";

        var action = () => builder.AddKeyedAzureTableClient(
            name,
            default(Action<AzureDataTablesSettings>?),
            default(Action<IAzureClientBuilder<TableServiceClient, TableClientOptions>>?));

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddKeyedAzureTableClientShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        var builder = new HostApplicationBuilder();
        var name = isNull ? null! : string.Empty;

        var action = () => builder.AddKeyedAzureTableClient(
            name,
            default(Action<AzureDataTablesSettings>?),
            default(Action<IAzureClientBuilder<TableServiceClient, TableClientOptions>>?));

        var exception = isNull ? Assert.Throws<ArgumentNullException>(action) : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}