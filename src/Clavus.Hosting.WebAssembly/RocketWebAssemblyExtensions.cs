using System.ComponentModel;
using Clavus.Hosting;
using Clavus.Hosting.WebAssembly;
using Clavus.Infrastructure;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Clavus;

[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ClavusWebAssemblyHelpers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<WebAssemblyHost> Configure(
        this WebAssemblyHostBuilder builder,
        Func<WebAssemblyHostBuilder, WebAssemblyHost> buildHost,
        ClavusContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(buildHost);
        ArgumentNullException.ThrowIfNull(contextBuilder);

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.HostEnvironment)
           .AddIfMissing(builder.HostEnvironment.GetType(), builder.HostEnvironment)
           .AddIfMissing("BaseAddress", builder.HostEnvironment.BaseAddress)
           .AddIfMissing("EnvironmentName", builder.HostEnvironment.Environment);
        contextBuilder.Properties.Add("BlazorWasm", true);

        var context = await ClavusContext.FromAsync(contextBuilder, cancellationToken).ConfigureAwait(false);

        await SharedHostConfigurationAsync(context, builder, cancellationToken).ConfigureAwait(false);
        await builder.Services.ApplyService(context, cancellationToken).ConfigureAwait(false);
        await builder.Logging.ApplyLogging(context, cancellationToken).ConfigureAwait(false);

        if (context.Get<ServiceProviderFactoryAdapter>() is { } factory)
            builder.ConfigureContainer(await factory(context, builder.Services, cancellationToken).ConfigureAwait(false));

        await builder.ApplyWebAssemblyHostBuilder(context, cancellationToken).ConfigureAwait(false);
        var host = buildHost(builder);
        await host.ApplyHostCreated(context, cancellationToken).ConfigureAwait(false);
        return host;
    }

    internal static ValueTask SharedHostConfigurationAsync(
        IClavusContext context,
        WebAssemblyHostBuilder builder,
        CancellationToken cancellationToken
    )
    {
        var configurationBuilder = (IConfigurationBuilder)builder.Configuration;

        // Clavus's own IConfigurationAsyncPart conventions (JsonBrowserConvention/YamlBrowserConvention/
        // TomlBrowserConvention, etc.) now own configuration loading end to end - including the HTTP fetch
        // that used to be Blazor's own default behavior for appsettings.json/appsettings.{Environment}.json -
        // so strip whatever the default WebAssembly host already loaded to avoid double-loading/precedence
        // conflicts, then insert a freshly-built ConfigurationBuilder in its place.
        foreach (var existing in configurationBuilder.Sources.OfType<JsonStreamConfigurationSource>().ToArray())
        {
            configurationBuilder.Sources.Remove(existing);
        }

        var cb = new ConfigurationBuilder();
        if (cb.Sources is { Count: > 0, })
        {
            configurationBuilder.Add(
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );
        }
        return cb.ApplyConfiguration(context, cancellationToken);
    }
}
