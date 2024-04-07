using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;

namespace Rocket.Surgery.WebAssembly.Hosting;

/// <summary>
///     Class RocketWebAssemblyExtensions.
/// </summary>
[PublicAPI]
public static class RocketWebAssemblyExtensions
{
    /// <summary>
    ///     Apply the conventions to the builder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionContext"></param>
    public static async ValueTask<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        IConventionContext conventionContext
    )
    {
        conventionContext.Properties.AddIfMissing<IConfiguration>(builder.Configuration);
        conventionContext.Properties.AddIfMissing(builder.HostEnvironment);
        conventionContext.Properties.Add("BlazorWasm", true);
        foreach (var item in conventionContext.Conventions
                                              .Get<IWebAssemblyHostingConvention, WebAssemblyHostingConvention, IWebAssemblyHostingAsyncConvention,
                                                   WebAssemblyHostingAsyncConvention>())
        {
            if (item is IWebAssemblyHostingConvention convention)
            {
                convention.Register(conventionContext, builder);
            }
            else if (item is WebAssemblyHostingConvention @delegate)
            {
                @delegate(conventionContext, builder);
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
                   .Select(z => getConfigurationSource(http, z))
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                   .Where(z => z is { })
                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   .Select(z => z!);

        foreach (var task in await Task.WhenAll(tasks))
        {
            if (task is null) continue;
            configurationBuilder.Add(task);
        }

        var cb = await new ConfigurationBuilder().ApplyConventionsAsync(conventionContext, builder.Configuration).ConfigureAwait(false);
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

        builder.ConfigureContainer(ConventionServiceProviderFactory.Wrap(conventionContext));
        return builder;

        static async Task<IConfigurationSource?> getConfigurationSource(HttpClient httpClient, ConfigurationBuilderDelegateResult factory)
        {
            IConfigurationSource? source = null;
            try
            {
                #pragma warning disable CA2234
                source = factory.Factory.Invoke(await httpClient.GetStreamAsync(factory.Path).ConfigureAwait(false));
                #pragma warning restore CA2234
            }
            catch (HttpRequestException) { }

            return source;
        }
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="builderAction"></param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static async ValueTask<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        Action<ConventionContextBuilder> builderAction
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var contextBuilder = new ConventionContextBuilder(null);
        builderAction.Invoke(contextBuilder);
        await ApplyConventions(builder, contextBuilder);

        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="appDomain"></param>
    /// <param name="getConventions"></param>
    /// <param name="action"></param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static ValueTask<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>>? getConventions = null,
        Action<ConventionContextBuilder>? action = null
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return ConfigureRocketSurgery(
            builder,
            a =>
            {
                a.UseAppDomain(appDomain);
                action?.Invoke(a);
                if (getConventions != null)
                {
                    a.WithConventionsFrom(getConventions);
                }
            }
        );
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions"></param>
    /// <param name="action"></param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static ValueTask<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>>? getConventions = null,
        Action<ConventionContextBuilder>? action = null
    )
    {
        return ConfigureRocketSurgery(builder, AppDomain.CurrentDomain, getConventions, action);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies"></param>
    /// <param name="getConventions"></param>
    /// <param name="action"></param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static ValueTask<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>>? getConventions = null,
        Action<ConventionContextBuilder>? action = null
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return ConfigureRocketSurgery(
            builder,
            a =>
            {
                a.UseAssemblies(assemblies);
                action?.Invoke(a);
                if (getConventions != null)
                {
                    a.WithConventionsFrom(getConventions);
                }
            }
        );
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static async Task<WebAssemblyHostBuilder> UseRocketBooster(
        this WebAssemblyHostBuilder builder,
        Func<WebAssemblyHostBuilder, ConventionContextBuilder> func,
        Action<ConventionContextBuilder>? action = null
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        var b = func(builder);
        action?.Invoke(b);
        await ApplyConventions(builder, b);
        return builder;
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static async Task<WebAssemblyHostBuilder> LaunchWith(
        this WebAssemblyHostBuilder builder,
        Func<WebAssemblyHostBuilder, ConventionContextBuilder> func,
        Action<ConventionContextBuilder>? action = null
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        var b = func(builder);
        action?.Invoke(b);
        await ApplyConventions(builder, b);
        return builder;
    }

    /// <summary>
    ///     Applys all conventions for hosting, configuration, services and logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="contextBuilder"></param>
    internal static async Task ApplyConventions(
        WebAssemblyHostBuilder builder,
        ConventionContextBuilder contextBuilder
    )
    {
        var context = ConventionContext.From(contextBuilder);
        await builder.ConfigureRocketSurgery(context);
    }

    /// <summary>
    ///     Applys all conventions for hosting, configuration, services and logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    internal static async Task ApplyConventions(
        WebAssemblyHostBuilder builder,
        IConventionContext context
    )
    {
        await builder.ConfigureRocketSurgery(context);
    }
}