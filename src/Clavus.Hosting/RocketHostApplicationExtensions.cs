using System.ComponentModel;
using Clavus.Hosting;
using Clavus.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Clavus;

[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ClavusHostApplicationHelpers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<THost> Configure<T, THost>(
        T builder,
        Func<T, THost> buildHost,
        ClavusContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
        where T : IHostApplicationBuilder
        where THost : IHost
    {
        ArgumentNullException.ThrowIfNull(buildHost);
        ArgumentNullException.ThrowIfNull(contextBuilder);

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing(builder.Configuration)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.Configuration.GetType(), builder.Configuration)
           .AddIfMissing(builder.Environment)
           .AddIfMissing(builder.Environment.GetType(), builder.Environment)
           .AddIfMissing("ApplicationName", builder.Environment.ApplicationName)
           .AddIfMissing("EnvironmentName", builder.Environment.EnvironmentName);

        var context = await ClavusContext.FromAsync(contextBuilder, cancellationToken).ConfigureAwait(false);
        await SharedHostConfigurationAsync(context, builder, cancellationToken).ConfigureAwait(false);
        await builder.Services.ApplyService(context, cancellationToken).ConfigureAwait(false);
        await builder.Logging.ApplyLogging(context, cancellationToken).ConfigureAwait(false);

        if (context.Get<ServiceProviderFactoryAdapter>() is { } factory)
            builder.ConfigureContainer(await factory(context, builder.Services, cancellationToken).ConfigureAwait(false));

        await builder.ApplyHostApplication(context, cancellationToken).ConfigureAwait(false);
        var host = buildHost(builder);
        await host.ApplyHostCreated(context, cancellationToken).ConfigureAwait(false);
        return host;
    }

    internal static ValueTask SharedHostConfigurationAsync(
        IClavusContext context,
        IHostApplicationBuilder hostApplicationBuilder,
        CancellationToken cancellationToken
    )
    {
        // Clavus's own IConfigurationPart conventions (JsonConvention/YamlConvention/TomlConvention, etc.) now
        // own file-based configuration loading end to end, so the host's own default file providers
        // (appsettings.json, appsettings.{Environment}.json, secrets.json, ...) would just be redundant/
        // conflicting - strip them before inserting Clavus's own configuration.
        foreach (var fileSource in hostApplicationBuilder.Configuration.Sources.OfType<FileConfigurationSource>().ToArray())
        {
            hostApplicationBuilder.Configuration.Sources.Remove(fileSource);
        }

        // Insert after everything that's left (e.g. anything the consumer added before calling into Clavus)
        // but before command line/environment variable configuration, so those retain the highest precedence.
        var insertIndex = hostApplicationBuilder.Configuration.Sources.Count;
        for (var i = 0; i < hostApplicationBuilder.Configuration.Sources.Count; i++)
        {
            var candidate = hostApplicationBuilder.Configuration.Sources[i];
            if (candidate is CommandLineConfigurationSource
             || ( candidate is EnvironmentVariablesConfigurationSource env
                 && ( string.IsNullOrWhiteSpace(env.Prefix) || string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) ))
            {
                insertIndex = i;
                break;
            }
        }
        var cb = new ConfigurationBuilder();
        if (cb.Sources is { Count: > 0, })
        {
            hostApplicationBuilder.Configuration.Sources.Insert(
                insertIndex,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );
        }

        hostApplicationBuilder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), }
        );
        return cb.ApplyConfiguration(context, cancellationToken);
    }
}
