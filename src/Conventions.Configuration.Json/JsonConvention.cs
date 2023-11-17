using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Json;

/// <summary>
/// Json configuraiton conventions
/// </summary>
[ExportConvention]
public class JsonConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        context.AppendApplicationConfiguration(
            configurationBuilder =>
            {
#if BROWSER
                return Array.Empty<ConfigurationBuilderDelegateResult>();
#else
                return new[]
                {
                    new ConfigurationBuilderDelegateResult("appsettings.json", LoadJsonFile(configurationBuilder, "appsettings.json"))
                };
#endif
            }
        );
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
#if BROWSER
                if (environment == "local")
                {
                    return new[]
                    {
                        new ConfigurationBuilderDelegateResult(
                            "appsettings.local.json",
                            stream => new JsonStreamConfigurationSource { Stream = stream ?? throw new ArgumentNullException(nameof(stream)) }
                        )
                    };
                }

                return Array.Empty<ConfigurationBuilderDelegateResult>();
#else
                return new[]
                {
                    new ConfigurationBuilderDelegateResult(
                        $"appsettings.{environment}.json",
                        LoadJsonFile(configurationBuilder, $"appsettings.{environment}.json")
                    )
                };
#endif
            }
        );
    }

#if !BROWSER
    private static Func<Stream?, IConfigurationSource> LoadJsonFile(IConfigurationBuilder configurationBuilder, string path)
    {
        return _ => new JsonConfigurationSource
        {
            Path = path,
            FileProvider = configurationBuilder.GetFileProvider(),
            ReloadOnChange = true,
            Optional = true
        };
    }
#endif
}
