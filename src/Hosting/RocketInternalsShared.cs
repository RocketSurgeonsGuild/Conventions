using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;

namespace Rocket.Surgery.Hosting;

internal static class RocketInternalsShared
{
    internal static async ValueTask SharedHostConfigurationAsync(
        IConventionContext context,
        IConfigurationBuilder configurationBuilder,
        IConfiguration configuration,
        IHostEnvironment environment,
        CancellationToken cancellationToken
    )
    {
        var index = SharedHostConfigurationInternal(context, configurationBuilder, configuration, environment);
        // Insert after all the normal configuration but before the environment specific configuration

        var cb = await new ConfigurationBuilder().ApplyConventionsAsync(context, configuration, cancellationToken).ConfigureAwait(false);
        if (cb.Sources is { Count: > 0, })
        {
            configurationBuilder.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );
        }
    }

    internal static void SharedHostConfiguration(
        IConventionContext context,
        IConfigurationBuilder configurationBuilder,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        var index = SharedHostConfigurationInternal(context, configurationBuilder, configuration, environment);
        // Insert after all the normal configuration but before the environment specific configuration

        var cb =  new ConfigurationBuilder().ApplyConventions(context, configuration);
        if (cb.Sources is { Count: > 0, })
        {
            configurationBuilder.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );
        }
    }

    private static int SharedHostConfigurationInternal(
        IConventionContext context,
        IConfigurationBuilder configurationBuilder,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        // This code is duplicated per host (web host, generic host, and wasm host)
        configurationBuilder.InsertConfigurationSourceAfter(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(
                               x.Path,
                               $"appsettings.{environment.EnvironmentName}.json",
                               StringComparison.OrdinalIgnoreCase
                           )
                       ),
            new IConfigurationSource[]
            {
                new JsonConfigurationSource
                {
                    FileProvider = configurationBuilder.GetFileProvider(),
                    Path = "appsettings.local.json",
                    Optional = true,
                    ReloadOnChange = true,
                },
            }
        );

        configurationBuilder.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
                       ),
            context
               .GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
               .SelectMany(z => z.Invoke(configurationBuilder))
               .Select(z => z.Factory(null))
        );

        if (!string.IsNullOrEmpty(environment.EnvironmentName))
        {
            configurationBuilder.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(
                               x => string.Equals(
                                   x.Path,
                                   $"appsettings.{environment.EnvironmentName}.json",
                                   StringComparison.OrdinalIgnoreCase
                               )
                           ),
                context
                   .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                   .SelectMany(z => z.Invoke(configurationBuilder, environment.EnvironmentName))
                   .Select(z => z.Factory(null))
            );
        }

        configurationBuilder.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(x => string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)),
            context
               .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
               .SelectMany(z => z.Invoke(configurationBuilder, "local"))
               .Select(z => z.Factory(null))
        );

        IConfigurationSource? source = null;
        foreach (var item in configurationBuilder.Sources.Reverse())
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

        var index = source == null
            ? configurationBuilder.Sources.Count - 1
            : configurationBuilder.Sources.IndexOf(source);

        return index;
    }
}
