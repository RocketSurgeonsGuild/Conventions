using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using AppDelegate =
    System.Func<Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ServiceFactoryAdapter =
    System.Func<Rocket.Surgery.Conventions.IConventionContext, Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<object>>>;

namespace Rocket.Surgery.WebAssembly.Hosting;

/// <summary>
///     Class RocketWebAssemblyExtensions.
/// </summary>
[PublicAPI]
public static class RocketWebAssemblyExtensions
{
    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<WebAssemblyHost> UseRocketBooster(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (func == null) throw new ArgumentNullException(nameof(func));
        var contextBuilder = await func(builder, cancellationToken);
        await action.Invoke(contextBuilder, cancellationToken);
        await Configure(builder, await ConventionContext.FromAsync(contextBuilder, cancellationToken), cancellationToken);
        return builder.Build();
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> UseRocketBooster(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        return UseRocketBooster(
            builder,
            func,
            (b, _) => action.Invoke(b),
            cancellationToken
        );
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> UseRocketBooster(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    )
    {
        return UseRocketBooster(
            builder,
            func,
            (b, _) =>
            {
                action.Invoke(b);
                return ValueTask.CompletedTask;
            },
            cancellationToken
        );
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> UseRocketBooster(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        CancellationToken cancellationToken = default
    )
    {
        return UseRocketBooster(builder, func, (_, _) => ValueTask.CompletedTask, cancellationToken);
    }


    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> LaunchWith(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    )
    {
        return UseRocketBooster(builder, func, action, cancellationToken);
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> LaunchWith(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        return UseRocketBooster(builder, func, action, cancellationToken);
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> LaunchWith(
        this WebAssemblyHostBuilder builder,
        AppDelegate func,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        return UseRocketBooster(builder, func, action, cancellationToken);
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> LaunchWith(this WebAssemblyHostBuilder builder, AppDelegate func, CancellationToken cancellationToken)
    {
        return UseRocketBooster(builder, func, cancellationToken);
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> LaunchWith(this WebAssemblyHostBuilder builder, AppDelegate func)
    {
        return UseRocketBooster(builder, func, CancellationToken.None);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> ConfigureRocketSurgery(this WebAssemblyHostBuilder builder, CancellationToken cancellationToken = default)
    {
        return ConfigureRocketSurgery(builder, _ => { }, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    )
    {
        return ConfigureRocketSurgery(
            builder,
            (b, _) =>
            {
                action(b);
                return ValueTask.CompletedTask;
            },
            cancellationToken
        );
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<WebAssemblyHost> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        return ConfigureRocketSurgery(builder, (b, ct) => action(b), cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<WebAssemblyHost> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object>());
        await action(contextBuilder, cancellationToken);
        await Configure(builder, await ConventionContext.FromAsync(contextBuilder, cancellationToken), cancellationToken);
        return builder.Build();
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostBuilder.</returns>
    public static async ValueTask<WebAssemblyHost> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        IConventionFactory getConventions,
        CancellationToken cancellationToken = default
    )
    {
        var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object>()).UseConventionFactory(getConventions);
        await Configure(builder, await ConventionContext.FromAsync(contextBuilder, cancellationToken), cancellationToken);
        return builder.Build();
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<WebAssemblyHost> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        ConventionContextBuilder conventionContextBuilder,
        CancellationToken cancellationToken = default
    )
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (conventionContextBuilder == null) throw new ArgumentNullException(nameof(conventionContextBuilder));
        await Configure(builder, await ConventionContext.FromAsync(conventionContextBuilder, cancellationToken), cancellationToken);
        return builder.Build();
    }

    /// <summary>
    ///     Apply the conventions to the builder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private static async ValueTask<WebAssemblyHostBuilder> Configure(
        this WebAssemblyHostBuilder builder,
        IConventionContext conventionContext,
        CancellationToken cancellationToken
    )
    {
        conventionContext
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.HostEnvironment)
           .AddIfMissing(builder.HostEnvironment.GetType(), builder.HostEnvironment);
        conventionContext.Properties.Add("BlazorWasm", true);
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
        {
            configurationBuilder.Add(
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true,
                }
            );
        }

        if (conventionContext.Get<ServiceFactoryAdapter>() is { } factory)
        {
            builder.ConfigureContainer(await factory(conventionContext, builder.Services, cancellationToken));
        }

        return builder;

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
}