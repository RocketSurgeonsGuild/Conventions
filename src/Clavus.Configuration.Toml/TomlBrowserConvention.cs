using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;


namespace Clavus.Configuration.Toml;

/// <summary>
///     Default toml convention
/// </summary>
[ClavusExport]
public class TomlBrowserConvention : IConfigurationAsyncPart
{
    /// <inheritdoc />
    public async ValueTask Register(IClavusContext context, IConfigurationBuilder builder, CancellationToken cancellationToken = default)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"))) return;
        var baseAddress = context.Get<string>("BaseAddress");
        if (baseAddress is not { Length: > 0, }) return;

        var applicationName = context.Get<string>("ApplicationName");
        var environmentName = context.Get<string>("EnvironmentName");

        using var http = new HttpClient { BaseAddress = new(baseAddress), };

        await AddTomlFile(http, builder, "appsettings.toml", cancellationToken).ConfigureAwait(false);
        if (applicationName is { Length: > 0, }) await AddTomlFile(http, builder, $"{applicationName}.toml", cancellationToken).ConfigureAwait(false);

        if (environmentName is { Length: > 0, })
        {
            await AddTomlFile(http, builder, $"appsettings.{environmentName}.toml", cancellationToken).ConfigureAwait(false);
            if (applicationName is { Length: > 0, })
                await AddTomlFile(http, builder, $"{applicationName}.{environmentName}.toml", cancellationToken).ConfigureAwait(false);
        }

        await AddTomlFile(http, builder, "appsettings.local.toml", cancellationToken).ConfigureAwait(false);
        if (applicationName is { Length: > 0, }) await AddTomlFile(http, builder, $"{applicationName}.local.toml", cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask AddTomlFile(HttpClient http, IConfigurationBuilder builder, string path, CancellationToken cancellationToken)
    {
        try
        {
            var stream = await http.GetStreamAsync(path, cancellationToken).ConfigureAwait(false);
            builder.AddTomlStream(stream);
        }
        catch (HttpRequestException) { }
    }
}
