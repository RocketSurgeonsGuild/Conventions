using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

[ExportConvention]
public class YamlConvention : ISetupConvention
{
    public void Register(IConventionContext context)
    {
        context.AddApplicationConfiguration(
            configurationBuilder =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")) || context.Properties.ContainsKey("BlazorWasm"))
                {
                    return new[]
                    {
                        new ConfigurationBuilderDelegateResult("appsettings.yaml", LoadBlazorWasmYamlFile),
                        new ConfigurationBuilderDelegateResult("appsettings.yml", LoadBlazorWasmYamlFile)
                    };
                }

                return new[]
                {
                    new ConfigurationBuilderDelegateResult("appsettings.yaml", LoadYamlFile(configurationBuilder, "appsettings.yaml")),
                    new ConfigurationBuilderDelegateResult("appsettings.yml", LoadYamlFile(configurationBuilder, "appsettings.yml"))
                };
            }
        );
        context.AddEnvironmentConfiguration(
            (configurationBuilder, environment) =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")) || context.Properties.ContainsKey("BlazorWasm"))
                {
                    return new[]
                    {
                        new ConfigurationBuilderDelegateResult($"appsettings.{environment}.yaml", LoadBlazorWasmYamlFile),
                        new ConfigurationBuilderDelegateResult($"appsettings.{environment}.yml", LoadBlazorWasmYamlFile)
                    };
                }

                return new[]
                {
                    new ConfigurationBuilderDelegateResult(
                        $"appsettings.{environment}.yaml",
                        LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yaml")
                    ),
                    new ConfigurationBuilderDelegateResult(
                        $"appsettings.{environment}.yml",
                        LoadYamlFile(configurationBuilder, $"appsettings.{environment}.yml")
                    )
                };
            }
        );
    }

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

    private static IConfigurationSource LoadBlazorWasmYamlFile(Stream? stream)
    {
        if (stream is null)
        {
            throw new NotSupportedException("Yaml is not supported without a stream");
        }

        return new YamlStreamConfigurationSource
        {
            Stream = stream
        };
    }
}
