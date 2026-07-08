using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;


namespace Clavus.Configuration.Toml;

/// <summary>
///     Default toml convention
/// </summary>
[ClavusExport]
public class TomlConvention : IConfigurationPart
{
    /// <inheritdoc />
    public void Register(IClavusContext context, IConfigurationBuilder builder)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
        var applicationName = context.Get<string>("ApplicationName");
        var environmentName = context.Get<string>("EnvironmentName");

        AddTomlFile(builder, "appsettings.toml");
        if (applicationName is { Length: > 0 }) AddTomlFile(builder, $"{applicationName}.toml");

        if (environmentName is { Length: > 0 })
        {
            AddTomlFile(builder, $"appsettings.{environmentName}.toml");
            if (applicationName is { Length: > 0 }) AddTomlFile(builder, $"{applicationName}.{environmentName}.toml");
        }

        AddTomlFile(builder, "appsettings.local.toml");
        if (applicationName is { Length: > 0 }) AddTomlFile(builder, $"{applicationName}.local.toml");
    }

    private static void AddTomlFile(IConfigurationBuilder builder, string path) =>
        builder.Add(
            new TomlConfigurationSource
            {
                Path = path,
                FileProvider = builder.GetFileProvider(),
                ReloadOnChange = true,
                Optional = true,
            }
        );
}
