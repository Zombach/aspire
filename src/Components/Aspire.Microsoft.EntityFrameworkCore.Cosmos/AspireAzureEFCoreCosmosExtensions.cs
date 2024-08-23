// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Aspire;
using Aspire.Hosting.Azure.Cosmos;
using Aspire.Microsoft.EntityFrameworkCore.Cosmos;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for configuring EntityFrameworkCore DbContext to Azure Cosmos DB
/// </summary>
public static class AspireAzureEFCoreCosmosExtensions
{
    private const string DefaultConfigSectionName = "Aspire:Microsoft:EntityFrameworkCore:Cosmos";
    private const DynamicallyAccessedMemberTypes RequiredByEF = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties;

    /// <summary>
    /// Registers the given <see cref="DbContext" /> as a service in the services provided by the <paramref name="builder"/>.
    /// Enables db context pooling, logging and telemetry.
    /// </summary>
    /// <typeparam name="TContext">The <see cref="DbContext" /> that needs to be registered.</typeparam>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="connectionName">A name used to retrieve the connection string from the ConnectionStrings configuration section.</param>
    /// <param name="databaseName">The name of the database to use within the Azure Cosmos DB account.</param>
    /// <param name="configureSettings">An optional delegate that can be used for customizing settings. It's invoked after the settings are read from the configuration.</param>
    /// <param name="configureDbContextOptions">An optional delegate to configure the <see cref="DbContextOptions"/> for the context.</param>
    /// <exception cref="ArgumentNullException">Thrown if mandatory <paramref name="builder"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when mandatory <see cref="EntityFrameworkCoreCosmosSettings.ConnectionString"/> is not provided.</exception>
    public static void AddCosmosDbContext<[DynamicallyAccessedMembers(RequiredByEF)] TContext>(
        this IHostApplicationBuilder builder,
        string connectionName,
        string databaseName,
        Action<EntityFrameworkCoreCosmosSettings>? configureSettings = null,
        Action<DbContextOptionsBuilder>? configureDbContextOptions = null) where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(connectionName);
        ArgumentException.ThrowIfNullOrEmpty(databaseName);

        builder.EnsureDbContextNotRegistered<TContext>();

        var settings = builder.GetDbContextSettings<TContext, EntityFrameworkCoreCosmosSettings>(
            DefaultConfigSectionName,
            (settings, section) => section.Bind(settings)
        );

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
        {
            if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
            {
                settings.AccountEndpoint = uri;
            }
            else
            {
                settings.ConnectionString = connectionString;
            }
        }

        configureSettings?.Invoke(settings);

        builder.Services.AddDbContextPool<TContext>(ConfigureDbContext);

        ConfigureInstrumentation<TContext>(builder, settings);

        void ConfigureDbContext(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!string.IsNullOrEmpty(settings.ConnectionString))
            {
                dbContextOptionsBuilder.UseCosmos(settings.ConnectionString, databaseName, UseCosmosBody);
            }
            else if (settings.AccountEndpoint is not null)
            {
                var credential = settings.Credential ?? new DefaultAzureCredential();
                dbContextOptionsBuilder.UseCosmos(settings.AccountEndpoint.OriginalString, credential, databaseName, UseCosmosBody);
            }
            else
            {
                throw new InvalidOperationException(
                  $"A DbContext could not be configured. Ensure valid connection information was provided in 'ConnectionStrings:{connectionName}' or either " +
                  $"{nameof(settings.ConnectionString)} or {nameof(settings.AccountEndpoint)} must be provided " +
                  $"in the '{DefaultConfigSectionName}' or '{DefaultConfigSectionName}:{typeof(TContext).Name}' configuration section.");
            }

            configureDbContextOptions?.Invoke(dbContextOptionsBuilder);
        }

        void UseCosmosBody(CosmosDbContextOptionsBuilder builder)
        {
            // We don't register logger factory, because there is no need to:
            // https://learn.microsoft.com/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.useloggerfactory?view=efcore-7.0#remarks
            if (settings.Region is not null)
            {
                builder.Region(settings.Region);
            }

            if (CosmosUtils.IsEmulatorConnectionString(settings.ConnectionString))
            {
                builder.ConnectionMode(ConnectionMode.Gateway);
                builder.LimitToEndpoint(true);
            }

            if (settings.RequestTimeout.HasValue)
            {
                builder.RequestTimeout(settings.RequestTimeout.Value);
            }
        }
    }

    /// <summary>
    /// Configures logging and telemetry for the <see cref="DbContext" />.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if mandatory <paramref name="builder"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when mandatory <see cref="DbContext"/> is not registered in DI.</exception>
    public static void EnrichCosmosDbContext<[DynamicallyAccessedMembers(RequiredByEF)] TContext>(
            this IHostApplicationBuilder builder,
            Action<EntityFrameworkCoreCosmosSettings>? configureSettings = null) where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = builder.GetDbContextSettings<TContext, EntityFrameworkCoreCosmosSettings>(
            DefaultConfigSectionName,
            (settings, section) => section.Bind(settings)
        );

        configureSettings?.Invoke(settings);

        if (settings.RequestTimeout.HasValue)
        {
            builder.PatchServiceDescriptor<TContext>(optionsBuilder =>
            {
#pragma warning disable EF1001 // Internal EF Core API usage.
                var extension = optionsBuilder.Options.FindExtension<CosmosOptionsExtension>();

                if (extension != null &&
                    extension.RequestTimeout.HasValue &&
                    extension.RequestTimeout != settings.RequestTimeout)
                {
                    throw new InvalidOperationException($"Conflicting values for 'RequestTimeout' were found in {nameof(EntityFrameworkCoreCosmosSettings)} and set in DbContextOptions<{typeof(TContext).Name}>.");
                }

                extension?.WithRequestTimeout(settings.RequestTimeout);
#pragma warning restore EF1001 // Internal EF Core API usage.
            });
        }
        else
        {
            builder.PatchServiceDescriptor<TContext>();
        }

        ConfigureInstrumentation<TContext>(builder, settings);
    }

    private static void ConfigureInstrumentation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TContext>(IHostApplicationBuilder builder, EntityFrameworkCoreCosmosSettings settings) where TContext : DbContext
    {
        if (!settings.DisableTracing)
        {
            builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder.AddSource("Azure.Cosmos.Operation");
            });
        }
    }
}
