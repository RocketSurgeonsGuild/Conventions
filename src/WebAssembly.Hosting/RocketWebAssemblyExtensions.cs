using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using ServiceFactoryAdapter =
    System.Func<Rocket.Surgery.Conventions.IConventionContext, Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<object>>>;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Rocket.Surgery.WebAssembly.Hosting;

[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RocketWebAssemblyExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConventionContextBuilder GetExisting(WebAssemblyHostBuilder builder)
    {
        _builder ??= new(new Dictionary<object, object>());
        return ImportHelpers.CallerConventions(Assembly.GetCallingAssembly()) is { } impliedFactory
            ? _builder.UseConventionFactory(impliedFactory)
            : _builder;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<WebAssemblyHost> Configure(
        this WebAssemblyHostBuilder builder,
        ConventionContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
    {
        if (contextBuilder.Properties.ContainsKey("__configured__")) throw new NotSupportedException("Cannot configure conventions on the same builder twice");
        contextBuilder.Properties["__configured__"] = true;

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.HostEnvironment)
           .AddIfMissing(builder.HostEnvironment.GetType(), builder.HostEnvironment);
        contextBuilder.Properties.Add("BlazorWasm", true);
        var conventionContext = await ConventionContext.FromAsync(contextBuilder, cancellationToken);
        foreach (var item in conventionContext.Conventions
                                              .Get<IWebAssemblyHostingConvention,
                                                   WebAssemblyHostingConvention,
                                                   IWebAssemblyHostingAsyncConvention,
                                                   WebAssemblyHostingAsyncConvention
                                               >())
        {
            switch (item)
            {
                case IWebAssemblyHostingConvention convention:
                    convention.Register(conventionContext, builder);
                    break;
                case WebAssemblyHostingConvention @delegate:
                    @delegate(conventionContext, builder);
                    break;
                case IWebAssemblyHostingAsyncConvention convention:
                    await convention.Register(conventionContext, builder, cancellationToken);
                    break;
                case WebAssemblyHostingAsyncConvention @delegate:
                    await @delegate(conventionContext, builder, cancellationToken);
                    break;
            }
        }

        var foundConfigurationFiles = Assembly
                                     .GetEntryAssembly()
                                    ?.GetCustomAttributes<AssemblyMetadataAttribute>()
                                     .Where(z => z.Key == "BlazorConfigurationFile")
                                      // ReSharper disable once NullableWarningSuppressionIsUsed
                                     .Select(z => z.Value!)
                                     .SelectMany(z => z.Split(';', StringSplitOptions.RemoveEmptyEntries))
                                     .ToHashSet(StringComparer.OrdinalIgnoreCase)
         ?? new();

        #pragma warning disable CA1859
        var configurationBuilder = (IConfigurationBuilder)builder.Configuration;
        #pragma warning restore CA1859
        using var http = new HttpClient
        {
            BaseAddress = new(builder.HostEnvironment.BaseAddress),
        };

        // notes to the next person that sees this.
        // if blazor does not find it's own configuration files (appsettings, appsettings.{environment}) they never get added to the configuration collection
        // in that case they never exist.  So unlike the other defaults where we have to replace the items.
        // If they exist then we just append.  If they don't we're adding anyway.
        // One place this might be an issue is if you have both appsettings.Development.json and appsettings.Development.yaml (or whatever)
        //   In this case the load order will be appsettings.json, appsettings.Development.json, appsettings.yaml, appsettings.Development.yaml
        //   Instead of the more desired order appsettings.json, appsettings.yaml, appsettings.Development.json, appsettings.Development.yaml
        // However this case is fairly rare as I would not expect an application to maintain both kinds of configuration files.

        var appTasks = conventionContext
                      .GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
                      .SelectMany(z => z.Invoke(configurationBuilder));

        var envTasks = conventionContext
                      .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                      .SelectMany(z => z.Invoke(configurationBuilder, builder.HostEnvironment.Environment));

        var localTasks = conventionContext
                        .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                        .SelectMany(z => z.Invoke(configurationBuilder, "local"))
                        .ToArray();

        var tasks = appTasks
                   .Concat(envTasks)
                   .Concat(localTasks)
                   .Where(z => foundConfigurationFiles.Contains(z.Path))
                   .Select(z => getConfigurationSource(http, z, cancellationToken))
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                   .Where(z => z is { })
                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   .Select(z => z!);

        foreach (var task in await Task.WhenAll(tasks))
        {
            if (task is null) continue;
            configurationBuilder.Add(task);
        }

        var cb = await new ConfigurationBuilder().ApplyConventionsAsync(conventionContext, builder.Configuration, cancellationToken).ConfigureAwait(false);
        if (cb.Sources is { Count: > 0, })
            configurationBuilder.Add(
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );

        if (conventionContext.Get<ServiceFactoryAdapter>() is { } factory)
            builder.ConfigureContainer(await factory(conventionContext, builder.Services, cancellationToken));

        return builder.Build();

        static async Task<IConfigurationSource?> getConfigurationSource(
            HttpClient httpClient,
            ConfigurationBuilderDelegateResult factory,
            CancellationToken cancellationToken
        )
        {
            IConfigurationSource? source = null;
            try
            {
                #pragma warning disable CA2234
                source = factory.Factory.Invoke(await httpClient.GetStreamAsync(factory.Path, cancellationToken).ConfigureAwait(false));
                #pragma warning restore CA2234
            }
            catch (HttpRequestException) { }

            return source;
        }
    }

    private static ConventionContextBuilder? _builder;
}
