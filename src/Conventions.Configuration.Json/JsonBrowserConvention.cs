using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration.Json;

using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Configuration.Json;

/// <summary>
///     Json configuration conventions
/// </summary>
[ExportConvention]
public class JsonBrowserConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
        context.AppendEnvironmentConfiguration(
            (configurationBuilder, environment) => environment == "local"
                ? new[]
                {
                    new ConfigurationBuilderDelegateResult(
                        "appsettings.local.json",
                        stream => new JsonStreamConfigurationSource { Stream = stream ?? throw new ArgumentNullException(nameof(stream)) }
                    ),
                }
                : Array.Empty<ConfigurationBuilderDelegateResult>()
        );
    }
}
