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
        context.AppendApplicationConfiguration(
            configurationBuilder => new[] { new ConfigurationBuilderDelegateResult("appsettings.json", LoadJsonFile(configurationBuilder, "appsettings.json")) }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) => new[]
            {
                new ConfigurationBuilderDelegateResult($"appsettings.{environment}.json", LoadJsonFile(configurationBuilder, $"appsettings.{environment}.json")),
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
