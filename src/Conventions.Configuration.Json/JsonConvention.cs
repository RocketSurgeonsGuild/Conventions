using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Json;

[ExportConvention]
public class JsonConvention : ISetupConvention
{
    public void Register(IConventionContext context)
    {
        context.AddApplicationConfiguration(
            configurationBuilder =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")) || context.Properties.ContainsKey("BlazorWasm"))
                {
                    return Array.Empty<ConfigurationBuilderDelegateResult>();
                }

                return new[]
                {
                    new ConfigurationBuilderDelegateResult("appsettings.json", LoadJsonFile(configurationBuilder, "appsettings.json"))
                };
            }
        );
        context.AddEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")) || context.Properties.ContainsKey("BlazorWasm"))
                {
                    return Array.Empty<ConfigurationBuilderDelegateResult>();
                }

                return new[]
                {
                    new ConfigurationBuilderDelegateResult(
                        $"appsettings.{environment}.json",
                        LoadJsonFile(configurationBuilder, $"appsettings.{environment}.json")
                    )
                };
            }
        );
    }

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
}
