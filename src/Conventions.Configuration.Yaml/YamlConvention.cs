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
#if BROWSER
                return new ConfigurationBuilderDelegateResult[]
                {
                    new("appsettings.yaml", LoadBlazorWasmYamlFile),
                    new("appsettings.yml", LoadBlazorWasmYamlFile)
                };
#else
                return new ConfigurationBuilderDelegateResult[]
                {
                    new ("appsettings.yaml", LoadYamlFile(configurationBuilder, "appsettings.yaml")),
                    new ("appsettings.yml", LoadYamlFile(configurationBuilder, "appsettings.yml"))
                };
#endif
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
#if BROWSER
                return new ConfigurationBuilderDelegateResult[]
                {
                    new($"appsettings.{environment}.yaml", LoadBlazorWasmYamlFile),
                    new($"appsettings.{environment}.yml", LoadBlazorWasmYamlFile)
                };
#else
                return new ConfigurationBuilderDelegateResult[]
                {
                    new ($"appsettings.{environment}.yaml",LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yaml")),
                    new ($"appsettings.{environment}.yml",LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yml"))
                };
#endif
            }
        );
    }

#if !BROWSER
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
#endif

#if BROWSER
    private static IConfigurationSource LoadBlazorWasmYamlFile(Stream? stream)
    {
        if (stream is null)
        {
            throw new NotSupportedException("Yaml is not supported without a stream");
        }

        return YamlConfigurationExtensions.CreateYamlConfigurationSource(stream);
    }
#endif
}
