using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Json;

/// <summary>
///     Json configuraiton conventions
/// </summary>
[ExportConvention]
public class JsonConvention : ISetupConvention
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
                    new ("appsettings.json", LoadJsonFile(configurationBuilder, "appsettings.json")),
                ];

#if  NET10_0_OR_GREATER
                return applicationName is {Length: > 0} ? [
                    ..results,
                    new ($"{applicationName}.json", LoadJsonFile(configurationBuilder, $"{applicationName}.json")),
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
                    new ($"appsettings.{environment}.json", LoadJsonFile(configurationBuilder, $"appsettings.{environment}.json")),
                ];

#if  NET10_0_OR_GREATER
                return applicationName is {Length: > 0} ? [
                    ..results,
                    new ($"{applicationName}.{environment}.json", LoadJsonFile(configurationBuilder, $"{applicationName}.{environment}.json")),
                ] : results;
#else
                return results;
#endif
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
