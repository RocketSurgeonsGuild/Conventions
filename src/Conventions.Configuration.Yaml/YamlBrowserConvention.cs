#if BROWSER
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     Default yaml convention
/// </summary>
[ExportConvention]
public class YamlBrowserConvention : ISetupConvention
{
    private static IConfigurationSource LoadBlazorWasmYamlFile(Stream? stream)
    {
        if (stream is null)
        {
            throw new NotSupportedException("Yaml is not supported without a stream");
        }

        return YamlConfigurationExtensions.CreateYamlConfigurationSource(stream);
    }

    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        context.AppendApplicationConfiguration(
            configurationBuilder =>
            {
                return new ConfigurationBuilderDelegateResult[]
                {
                    new("appsettings.yaml", LoadBlazorWasmYamlFile),
                    new("appsettings.yml", LoadBlazorWasmYamlFile),
                };
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                return new ConfigurationBuilderDelegateResult[]
                {
                    new($"appsettings.{environment}.yaml", LoadBlazorWasmYamlFile),
                    new($"appsettings.{environment}.yml", LoadBlazorWasmYamlFile),
                };
            }
        );
    }
}
#endif