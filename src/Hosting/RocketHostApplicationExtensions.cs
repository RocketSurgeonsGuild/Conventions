using System.ComponentModel;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.Extensions;

#pragma warning disable CA1031, CA2000, CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Hosting;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RocketHostApplicationExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<THost> Configure<T, THost>(
        T builder,
        Func<T, THost> buildHost,
        ConventionContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
        where T : IHostApplicationBuilder
        where THost : IHost
    {
        ArgumentNullException.ThrowIfNull(buildHost);
        ArgumentNullException.ThrowIfNull(contextBuilder);

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing(builder.Configuration)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.Configuration.GetType(), builder.Configuration)
           .AddIfMissing(builder.Environment)
           .AddIfMissing(nameof(builder.Environment.ApplicationName), builder.Environment.ApplicationName)
           .AddIfMissing(nameof(builder.Environment.ContentRootPath), builder.Environment.ContentRootPath)
           .AddIfMissing(nameof(builder.Environment.EnvironmentName), builder.Environment.EnvironmentName)
           .AddIfMissing(builder.Environment.GetType(), builder.Environment);

        var context = await ConventionContext.FromAsync(contextBuilder, cancellationToken).ConfigureAwait(false);
        await SharedHostConfigurationAsync(context, builder, cancellationToken).ConfigureAwait(false);
        await builder.Services.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);
        await builder.Logging.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);

        if (context.Get<ServiceProviderFactoryAdapter>() is { } factory)
            builder.ConfigureContainer(await factory(context, builder.Services, cancellationToken).ConfigureAwait(false));

        await builder.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);
        var host = buildHost(builder);
        await context.ApplyHostCreatedConventionsAsync(host, cancellationToken).ConfigureAwait(false);
        return host;
    }

    internal static async ValueTask SharedHostConfigurationAsync(
        IConventionContext context,
        IHostApplicationBuilder hostApplicationBuilder,
        CancellationToken cancellationToken
    )
    {
        // This code is duplicated per host (web host, generic host, and wasm host)
        void insertNamedSource(string name)
        {
            hostApplicationBuilder.Configuration.InsertConfigurationSourceAfter(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(x => string.Equals(
                                              x.Path,
                                              $"{name}.{hostApplicationBuilder.Environment.EnvironmentName}.json",
                                              StringComparison.OrdinalIgnoreCase
                                          )
                           ),
                new IConfigurationSource[]
                {
                    new JsonConfigurationSource
                    {
                        FileProvider = hostApplicationBuilder.Configuration.GetFileProvider(),
                        Path = $"{name}.local.json",
                        Optional = true,
                        ReloadOnChange = true,
                    },
                }
            );

            hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(x => string.Equals(x.Path, $"{name}.json", StringComparison.OrdinalIgnoreCase)
                           ),
                context
                   .GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => [])
                   .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration))
                   .Select(z => z.Factory(null))
            );

            hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(x => string.Equals(
                                              x.Path,
                                              $"{name}.{hostApplicationBuilder.Environment.EnvironmentName}.json",
                                              StringComparison.OrdinalIgnoreCase
                                          )
                           ),
                context
                   .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => [])
                   .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration, hostApplicationBuilder.Environment.EnvironmentName))
                   .Select(z => z.Factory(null))
            );

            hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(x => string.Equals(x.Path, $"{name}.local.json", StringComparison.OrdinalIgnoreCase)),
                context
                   .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => [])
                   .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration, "local"))
                   .Select(z => z.Factory(null))
            );
        }

        insertNamedSource("appsettings");
        insertNamedSource(hostApplicationBuilder.Environment.ApplicationName);

        IConfigurationSource? source = null;
        foreach (var item in hostApplicationBuilder.Configuration.Sources.Reverse())
        {
            if (item is CommandLineConfigurationSource
             || ( item is EnvironmentVariablesConfigurationSource env
                 && ( string.IsNullOrWhiteSpace(env.Prefix) || string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) )
             || ( item is FileConfigurationSource a && string.Equals(a.Path, "secrets.json", StringComparison.OrdinalIgnoreCase) ))
            {
                continue;
            }

            source = item;
            break;
        }

        var index = source is null
            ? hostApplicationBuilder.Configuration.Sources.Count - 1
            : hostApplicationBuilder.Configuration.Sources.IndexOf(source);
        // Insert after all the normal configuration but before the environment specific configuration

        var cb = await new ConfigurationBuilder().ApplyConventionsAsync(context, hostApplicationBuilder.Configuration, cancellationToken).ConfigureAwait(false);
        if (cb.Sources is { Count: > 0, })
        {
            hostApplicationBuilder.Configuration.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );
        }

        hostApplicationBuilder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), }
        );
    }
}
