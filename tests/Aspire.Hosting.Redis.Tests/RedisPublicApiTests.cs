// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Utils;
using Xunit;

namespace Aspire.Hosting.Redis.Tests;

public class RedisPublicApiTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void AddRedisShouldThrowWhenBuilderIsNull(int overrideIndex)
    {
        IDistributedApplicationBuilder builder = null!;
        const string name = "Redis";
        int? port = null;
        IResourceBuilder<ParameterResource>? password = null;

        Action action = overrideIndex switch
        {
            0 => () => builder.AddRedis(name, port),
            1 => () => builder.AddRedis(name, port, password),
            _ => throw new InvalidOperationException()
        };

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(1, true)]
    public void AddRedisShouldThrowWhenNameIsNullOrEmpty(int overrideIndex, bool isNull)
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var name = isNull ? null! : string.Empty;
        int? port = null;
        IResourceBuilder<ParameterResource>? password = null;

        Action action = overrideIndex switch
        {
            0 => () => builder.AddRedis(name, port),
            1 => () => builder.AddRedis(name, port, password),
            _ => throw new InvalidOperationException()
        };

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }

    [Fact]
    public void WithRedisCommanderShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisResource> builder = null!;

        var action = () => builder.WithRedisCommander();

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void WithRedisInsightShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisResource> builder = null!;

        var action = () => builder.WithRedisInsight();

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void RedisCommanderWithHostPortShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisCommanderResource> builder = null!;
        const int port = 777;

        var action = () => builder.WithHostPort(port);

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void RedisInsightWithHostPortShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisInsightResource> builder = null!;
        const int port = 777;

        var action = () => builder.WithHostPort(port);

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void WithDataVolumeShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisResource> builder = null!;

        var action = () => builder.WithDataVolume();

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void WithDataBindMountShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisResource> builder = null!;
        const string source = "/data";

        var action = () => builder.WithDataBindMount(source);

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithDataBindMountShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        var redis = builder.AddRedis("Redis");
        var source = isNull ? null! : string.Empty;

        var action = () => redis.WithDataBindMount(source);

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(source), exception.ParamName);
    }

    [Fact]
    public void WithPersistenceShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisResource> builder = null!;

        var action = () => builder.WithPersistence();

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void RedisInsightWithDataVolumeShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisInsightResource> builder = null!;

        var action = () => builder.WithDataVolume();

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Fact]
    public void RedisInsightWithDataBindMountShouldThrowWhenBuilderIsNull()
    {
        IResourceBuilder<RedisInsightResource> builder = null!;
        const string source = "/data";

        var action = () => builder.WithDataBindMount(source);

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(builder), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RedisInsightWithDataBindMountShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        using var builder = TestDistributedApplicationBuilder.Create();
        IResourceBuilder<RedisInsightResource>? redisInsightBuilder = null;
        var redis = builder.AddRedis("Redis").WithRedisInsight(resource => { redisInsightBuilder = resource; });
        var source = isNull ? null! : string.Empty;

        Assert.NotNull(redisInsightBuilder);
        var action = () => redisInsightBuilder.WithDataBindMount(source);

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(source), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CtorRedisCommanderResourceShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        var name = isNull ? null! : string.Empty;

        var action = () => new RedisCommanderResource(name);

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CtorRedisInsightResourceShouldThrowWhenNameIsNullOrEmpty(bool isNull)
    {
        var name = isNull ? null! : string.Empty;

        var action = () => new RedisInsightResource(name);

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }

    [Fact]
    public void CtorRedisResourceShouldThrowWhenPasswordIsNull()
    {
        const string name = "redis";
        ParameterResource password = null!;

        var action = () => new RedisResource(name, password);

        var exception = Assert.Throws<ArgumentNullException>(action);
        Assert.Equal(nameof(password), exception.ParamName);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(1, true)]
    public void CtorRedisResourceShouldThrowWhenNameIsNullOrEmpty(int overrideIndex, bool isNull)
    {
        var name = isNull ? null! : string.Empty;
        var password = new ParameterResource("password", (_) => "password");

        Action action = overrideIndex switch
        {
            0 => () => new RedisResource(name),
            1 => () => new RedisResource(name, password),
            _ => throw new InvalidOperationException()
        };

        var exception = isNull
            ? Assert.Throws<ArgumentNullException>(action)
            : Assert.Throws<ArgumentException>(action);
        Assert.Equal(nameof(name), exception.ParamName);
    }
}
