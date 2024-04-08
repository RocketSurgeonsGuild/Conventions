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
        IHostApplicationBuilder hostApplicationBuilder,
        CancellationToken cancellationToken
    )
    {
        // This code is duplicated per host (web host, generic host, and wasm host)
        hostApplicationBuilder.Configuration.InsertConfigurationSourceAfter(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(
                               x.Path,
                               $"appsettings.{hostApplicationBuilder.Environment.EnvironmentName}.json",
                               StringComparison.OrdinalIgnoreCase
                           )
                       ),
            new IConfigurationSource[]
            {
                new JsonConfigurationSource
                {
                    FileProvider = hostApplicationBuilder.Configuration.GetFileProvider(),
                    Path = "appsettings.local.json",
                    Optional = true,
                    ReloadOnChange = true,
                },
            }
        );

        hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
                       ),
            context
               .GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
               .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration))
               .Select(z => z.Factory(null))
        );

        if (!string.IsNullOrEmpty(hostApplicationBuilder.Environment.EnvironmentName))
        {
            hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(
                               x => string.Equals(
                                   x.Path,
                                   $"appsettings.{hostApplicationBuilder.Environment.EnvironmentName}.json",
                                   StringComparison.OrdinalIgnoreCase
                               )
                           ),
                context
                   .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                   .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration, hostApplicationBuilder.Environment.EnvironmentName))
                   .Select(z => z.Factory(null))
            );
        }

        hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(x => string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)),
            context
               .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
               .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration, "local"))
               .Select(z => z.Factory(null))
        );

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

        var index = source == null
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
    }
}