using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.Extensions;

#pragma warning disable CA1031
#pragma warning disable CA2000
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Hosting;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RocketHostApplicationExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConventionContextBuilder GetExisting(IHostApplicationBuilder builder)
    {
        var contextBuilder = builder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var conventionContextBuilder)
         && conventionContextBuilder is ConventionContextBuilder b
                ? b
                : new(new Dictionary<object, object>());
        var ccb = ImportHelpers.CallerConventions(Assembly.GetCallingAssembly()) is { } impliedFactory
            ? contextBuilder.UseConventionFactory(impliedFactory)
            : contextBuilder;
        builder.Properties[typeof(ConventionContextBuilder)] = ccb;
        return ccb;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<THost> Configure<T, THost>(
        T builder,
        Func<T, THost> buildHost,
        ConventionContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
        where T : IHostApplicationBuilder
        where THost : IHost
    {
        if (contextBuilder.Properties.ContainsKey("__configured__")) throw new NotSupportedException("Cannot configure conventions on the same builder twice");
        contextBuilder.Properties["__configured__"] = true;

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing(builder.Configuration)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.Configuration.GetType(), builder.Configuration)
           .AddIfMissing(builder.Environment)
           .AddIfMissing(builder.Environment.GetType(), builder.Environment);

        var context = await ConventionContext.FromAsync(contextBuilder, cancellationToken);
        await SharedHostConfigurationAsync(context, builder, cancellationToken).ConfigureAwait(false);
        await builder.Services.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);
        await builder.Logging.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);

        if (context.Get<ServiceProviderFactoryAdapter>() is { } factory)
            builder.ConfigureContainer(await factory(context, builder.Services, cancellationToken));

        await builder.ApplyConventionsAsync(context, cancellationToken);
        var host = buildHost(builder);
        await context.ApplyHostCreatedConventionsAsync(host, cancellationToken);
        return host;
    }

    internal static async ValueTask SharedHostConfigurationAsync(
        IConventionContext context,
        IHostApplicationBuilder hostApplicationBuilder,
        CancellationToken cancellationToken
    )
    {
        // This code is duplicated per host (web host, generic host, and wasm host)
        hostApplicationBuilder.Configuration.InsertConfigurationSourceAfter(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(
                               x.Path,
                               $"appsettings.{hostApplicationBuilder.Environment.EnvironmentName}.json",
                               StringComparison.OrdinalIgnoreCase
                           )
                       ),
            new IConfigurationSource[]
            {
                new JsonConfigurationSource
                {
                    FileProvider = hostApplicationBuilder.Configuration.GetFileProvider(),
                    Path = "appsettings.local.json",
                    Optional = true,
                    ReloadOnChange = true,
                },
            }
        );

        hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
                       ),
            context
               .GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
               .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration))
               .Select(z => z.Factory(null))
        );

        if (!string.IsNullOrEmpty(hostApplicationBuilder.Environment.EnvironmentName))
            hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(
                               x => string.Equals(
                                   x.Path,
                                   $"appsettings.{hostApplicationBuilder.Environment.EnvironmentName}.json",
                                   StringComparison.OrdinalIgnoreCase
                               )
                           ),
                context
                   .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                   .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration, hostApplicationBuilder.Environment.EnvironmentName))
                   .Select(z => z.Factory(null))
            );

        hostApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(x => string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)),
            context
               .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
               .SelectMany(z => z.Invoke(hostApplicationBuilder.Configuration, "local"))
               .Select(z => z.Factory(null))
        );

        IConfigurationSource? source = null;
        foreach (var item in hostApplicationBuilder.Configuration.Sources.Reverse())
        {
            if (item is CommandLineConfigurationSource
             || ( item is EnvironmentVariablesConfigurationSource env
                 && ( string.IsNullOrWhiteSpace(env.Prefix) || string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) )
             || ( item is FileConfigurationSource a && string.Equals(a.Path, "secrets.json", StringComparison.OrdinalIgnoreCase) ))
                continue;

            source = item;
            break;
        }

        var index = source == null
            ? hostApplicationBuilder.Configuration.Sources.Count - 1
            : hostApplicationBuilder.Configuration.Sources.IndexOf(source);
        // Insert after all the normal configuration but before the environment specific configuration

        var cb = await new ConfigurationBuilder().ApplyConventionsAsync(context, hostApplicationBuilder.Configuration, cancellationToken).ConfigureAwait(false);
        if (cb.Sources is { Count: > 0, })
            hostApplicationBuilder.Configuration.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );

        hostApplicationBuilder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), }
        );
    }
}
