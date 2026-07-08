using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


namespace Clavus.Configuration.Json;

/// <summary>
///     Json configuration conventions
/// </summary>
[ClavusExport]
public class JsonBrowserConvention : IConfigurationAsyncPart
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

        await AddJsonFile(http, builder, "appsettings.json", cancellationToken).ConfigureAwait(false);
        if (applicationName is { Length: > 0, }) await AddJsonFile(http, builder, $"{applicationName}.json", cancellationToken).ConfigureAwait(false);

        if (environmentName is { Length: > 0, })
        {
            await AddJsonFile(http, builder, $"appsettings.{environmentName}.json", cancellationToken).ConfigureAwait(false);
            if (applicationName is { Length: > 0, })
                await AddJsonFile(http, builder, $"{applicationName}.{environmentName}.json", cancellationToken).ConfigureAwait(false);
        }

        await AddJsonFile(http, builder, "appsettings.local.json", cancellationToken).ConfigureAwait(false);
        if (applicationName is { Length: > 0, }) await AddJsonFile(http, builder, $"{applicationName}.local.json", cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask AddJsonFile(HttpClient http, IConfigurationBuilder builder, string path, CancellationToken cancellationToken)
    {
        try
        {
            var stream = await http.GetStreamAsync(path, cancellationToken).ConfigureAwait(false);
            builder.Add(new JsonStreamConfigurationSource { Stream = stream, });
        }
        catch (HttpRequestException) { }
    }
}
