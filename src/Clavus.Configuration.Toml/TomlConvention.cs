using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;


namespace Clavus.Configuration.Toml;

/// <summary>
///     Default toml convention
/// </summary>
[ClavusExport]
public class TomlConvention : ISetupPart
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
                    new("appsettings.toml", LoadTomlFile(configurationBuilder, "appsettings.toml")),
                ];

                return applicationName is { Length: > 0 } ? [
                    ..results,
                    new($"{applicationName}.toml", LoadTomlFile(configurationBuilder, $"{applicationName}.toml")),
                ] : results;
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                ConfigurationBuilderDelegateResult[] results =
                [
                    new($"appsettings.{environment}.toml", LoadTomlFile(configurationBuilder, $"appsettings.{environment}.toml")),
                ];

                return applicationName is { Length: > 0 } ? [
                    ..results,
                    new($"{applicationName}.{environment}.toml", LoadTomlFile(configurationBuilder, $"{applicationName}.{environment}.toml")),
                ] : results;
            }
        );
    }

    private static Func<Stream?, IConfigurationSource> LoadTomlFile(IConfigurationBuilder configurationBuilder, string path) => _ => new TomlConfigurationSource
    {
        Path = path,
        FileProvider = configurationBuilder.GetFileProvider(),
        ReloadOnChange = true,
        Optional = true,
    };
}
