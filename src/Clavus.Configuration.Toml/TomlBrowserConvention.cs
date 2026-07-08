using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;


namespace Clavus.Configuration.Toml;

/// <summary>
///     Default toml convention
/// </summary>
[ClavusExport]
public class TomlBrowserConvention : ISetupPart
{
    /// <inheritdoc />
    public void Register(IClavusContext context)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
        context.AppendApplicationConfiguration(
            configurationBuilder =>
            {
                return new ConfigurationBuilderDelegateResult[]
                {
                    new("appsettings.toml", LoadBlazorWasmTomlFile),
                };
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                return new ConfigurationBuilderDelegateResult[]
                {
                    new($"appsettings.{environment}.toml", LoadBlazorWasmTomlFile),
                };
            }
        );
    }

    private static IConfigurationSource LoadBlazorWasmTomlFile(Stream? stream) => stream is null
        ? throw new NotSupportedException("Toml is not supported without a stream")
        : TomlConfigurationExtensions.CreateTomlConfigurationSource(stream);
}
