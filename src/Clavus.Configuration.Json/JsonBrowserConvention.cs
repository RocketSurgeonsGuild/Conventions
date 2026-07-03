using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration.Json;


namespace Clavus.Configuration.Json;

/// <summary>
///     Json configuration conventions
/// </summary>
[ExportClavusPart]
public class JsonBrowserConvention : ISetupPart
{
    /// <inheritdoc />
    public void Register(IClavusContext context)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) => environment == "local"
                ?
                [
                    new(
                        "appsettings.local.json",
                        stream => new JsonStreamConfigurationSource { Stream = stream ?? throw new ArgumentNullException(nameof(stream)) }
                    ),
                ]
                : Array.Empty<ConfigurationBuilderDelegateResult>()
        );
    }
}
