using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


namespace Clavus.Configuration.Json;

/// <summary>
///     Json configuraiton conventions
/// </summary>
[ClavusExport]
public class JsonConvention : ISetupPart
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
                    new ("appsettings.json", LoadJsonFile(configurationBuilder, "appsettings.json")),
                ];

                return applicationName is { Length: > 0 } ? [
                    ..results,
                    new ($"{applicationName}.json", LoadJsonFile(configurationBuilder, $"{applicationName}.json")),
                ] : results;
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                ConfigurationBuilderDelegateResult[] results =
                [
                    new ($"appsettings.{environment}.json", LoadJsonFile(configurationBuilder, $"appsettings.{environment}.json")),
                ];

                return applicationName is { Length: > 0 } ? [
                    ..results,
                    new ($"{applicationName}.{environment}.json", LoadJsonFile(configurationBuilder, $"{applicationName}.{environment}.json")),
                ] : results;
            }
        );
    }

    private static Func<Stream?, IConfigurationSource> LoadJsonFile(IConfigurationBuilder configurationBuilder, string path) => _ => new JsonConfigurationSource
    {
        Path = path,
        FileProvider = configurationBuilder.GetFileProvider(),
        ReloadOnChange = true,
        Optional = true,
    };
}
