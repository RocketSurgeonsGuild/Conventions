using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;


namespace Clavus.Configuration.Yaml;

/// <summary>
///     Default yaml convention
/// </summary>
[ClavusExport]
public class YamlConvention : ISetupPart
{
    /// <inheritdoc />
    public void Register(IClavusContext context)
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

                return applicationName is { Length: > 0 } ? [
                    ..results,
                    new($"{applicationName}.yaml", LoadYamlFile(configurationBuilder, $"{applicationName}.yaml")),
                    new($"{applicationName}.yml", LoadYamlFile(configurationBuilder, $"{applicationName}.yml")),
                ] : results;
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

                return applicationName is { Length: > 0 } ? [
                    ..results,
                    new($"{applicationName}.{environment}.yaml", LoadYamlFile(configurationBuilder, $"{applicationName}.{environment}.yaml")),
                    new($"{applicationName}.{environment}.yml", LoadYamlFile(configurationBuilder, $"{applicationName}.{environment}.yml")),
                ] : results;
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
