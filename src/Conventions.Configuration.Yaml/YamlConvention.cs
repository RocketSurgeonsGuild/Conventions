using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     Default yaml convention
/// </summary>
[ExportConvention]
public class YamlConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
        var applicationName = context.Get<string>("ApplicationName");
        context.AppendApplicationConfiguration(
            configurationBuilder =>
            {
                ConfigurationBuilderDelegateResult[] results =
                [
                    new ("appsettings.yaml", LoadYamlFile(configurationBuilder, "appsettings.yaml")),
                    new ("appsettings.yml", LoadYamlFile(configurationBuilder, "appsettings.yml"))
                ];


<<<<<<< TODO: Unmerged change from project 'Rocket.Surgery.Conventions.Configuration.Yaml(net10.0)', Before:
                #if  NET10_0_OR_GREATER
                return applicationName is {Length: > 0} ? [
=======
#if NET10_0_OR_GREATER
                return applicationName is { Length: > 0 } ? [
>>>>>>> After

<<<<<<< TODO: Unmerged change from project 'Rocket.Surgery.Conventions.Configuration.Yaml(net10.0)', Before:
                #else
                return results;
                #endif
=======
#else
                return results;
#endif
>>>>>>> After
#if NET10_0_OR_GREATER
                return applicationName is {Length: > 0} ? [
                    ..results,
                    new($"{applicationName}.yaml", LoadYamlFile(configurationBuilder, $"{applicationName}.yaml")),
                    new($"{applicationName}.yml", LoadYamlFile(configurationBuilder, $"{applicationName}.yml")),
                ] : results;
#else
                return results;
#endif
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                ConfigurationBuilderDelegateResult[] results =
                [
                    new($"appsettings.{environment}.yaml", LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yaml")),
                    new($"appsettings.{environment}.yml", LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yml")),
                ];


<<<<<<< TODO: Unmerged change from project 'Rocket.Surgery.Conventions.Configuration.Yaml(net10.0)', Before:
                #if  NET10_0_OR_GREATER
                return applicationName is {Length: > 0} ? [
=======
#if NET10_0_OR_GREATER
                return applicationName is { Length: > 0 } ? [
>>>>>>> After

<<<<<<< TODO: Unmerged change from project 'Rocket.Surgery.Conventions.Configuration.Yaml(net10.0)', Before:
                #else
                return results;
                #endif
=======
#else
                return results;
#endif
>>>>>>> After
#if NET10_0_OR_GREATER
                return applicationName is {Length: > 0} ? [
                    ..results,
                    new($"{applicationName}.{environment}.yaml", LoadYamlFile(configurationBuilder, $"{applicationName}.{environment}.yaml")),
                    new($"{applicationName}.{environment}.yml", LoadYamlFile(configurationBuilder, $"{applicationName}.{environment}.yml")),
                ] : results;
#else
                return results;
#endif
            }
        );
    }

    private static Func<Stream?, IConfigurationSource> LoadYamlFile(IConfigurationBuilder configurationBuilder, string path) => _ => new YamlConfigurationSource
    {
        Path = path,
        FileProvider = configurationBuilder.GetFileProvider(),
        ReloadOnChange = true,
        Optional = true,
    };
}
