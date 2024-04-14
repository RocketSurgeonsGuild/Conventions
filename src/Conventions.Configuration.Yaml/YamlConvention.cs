#if !BROWSER
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
/// Default yaml convention
/// </summary>
[ExportConvention]
public class YamlConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        context.AppendApplicationConfiguration(
            configurationBuilder =>
            {
                return new ConfigurationBuilderDelegateResult[]
                {
                    new ("appsettings.yaml", LoadYamlFile(configurationBuilder, "appsettings.yaml")),
                    new ("appsettings.yml", LoadYamlFile(configurationBuilder, "appsettings.yml"))
                };
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                return new ConfigurationBuilderDelegateResult[]
                {
                    new ($"appsettings.{environment}.yaml",LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yaml")),
                    new ($"appsettings.{environment}.yml",LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yml"))
                };
            }
        );
    }

    private static Func<Stream?, IConfigurationSource> LoadYamlFile(IConfigurationBuilder configurationBuilder, string path)
    {
        return _ => new YamlConfigurationSource
        {
            Path = path,
            FileProvider = configurationBuilder.GetFileProvider(),
            ReloadOnChange = true,
            Optional = true
        };
    }
}
#endif
