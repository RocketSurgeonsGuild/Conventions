using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     Default yaml convention
/// </summary>
[ExportConvention]
public class YamlBrowserConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
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

    private static IConfigurationSource LoadBlazorWasmYamlFile(Stream? stream) => stream is null
        ? throw new NotSupportedException("Yaml is not supported without a stream")
        : YamlConfigurationExtensions.CreateYamlConfigurationSource(stream);
}
