using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;

#pragma warning disable CA1031
#pragma warning disable CA2000

namespace Rocket.Surgery.WebAssembly.Hosting;

/// <summary>
///     Class RocketWebAssemblyExtensions.
/// </summary>
public static class RocketWebAssemblyExtensions
{
    /// <summary>
    ///     Apply the conventions to the builder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionContext"></param>
    public static Task<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        IConventionContext conventionContext
    )
    {
        conventionContext.Properties.AddIfMissing<IConfiguration>(builder.Configuration);
        conventionContext.Properties.AddIfMissing(builder.HostEnvironment);
        conventionContext.Properties.Add("BlazorWasm", true);
        foreach (var item in conventionContext.Conventions.Get<IWebAssemblyHostingConvention, WebAssemblyHostingConvention>())
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

        var jsRuntime = (IJSUnmarshalledRuntime)builder.Services.First(z => z.ServiceType == typeof(IJSRuntime)).ImplementationInstance!;

        static IConfigurationSource? GetConfigurationSource(IJSUnmarshalledRuntime runtime, ConfigurationBuilderDelegateResult factory)
        {
            IConfigurationSource? source = null;
            try
            {
                var content = runtime.InvokeUnmarshalled<string, byte[]>("Blazor._internal.getConfig", factory.Path);
                source = factory.Factory(new MemoryStream(content));
            }
            catch (HttpRequestException)
            {
            }

            return source;
        }

        var configurationBuilder = (IConfigurationBuilder)builder.Configuration;

        var localTask = GetConfigurationSource(
            jsRuntime, new ConfigurationBuilderDelegateResult("appsettings.local.json", stream => new JsonStreamConfigurationSource { Stream = stream })
        );

        var appTasks = conventionContext
                      .GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
                      .SelectMany(z => z.Invoke(configurationBuilder))
                      .Select(z => GetConfigurationSource(jsRuntime, z))
                      .Where(z => z is not null)
                      .ToArray();

        var envTasks = conventionContext
                      .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                      .SelectMany(z => z.Invoke(configurationBuilder, builder.HostEnvironment.Environment))
                      .Select(z => GetConfigurationSource(jsRuntime, z))
                      .Where(z => z is not null)
                      .ToArray();

        var localTasks = conventionContext
                        .GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                        .SelectMany(z => z.Invoke(configurationBuilder, "local"))
                        .Select(z => GetConfigurationSource(jsRuntime, z))
                        .Where(z => z is not null)
                        .ToArray();

        configurationBuilder.Add(localTask);

        // [0] is appsettings.json
        foreach (var result in appTasks)
        {
            configurationBuilder.Add(result);
        }

        // [1] is appsettings.{Environment}.json
        foreach (var result in envTasks)
        {
            configurationBuilder.Add(result);
        }

        foreach (var result in localTasks)
        {
            configurationBuilder.Add(result);
        }

        var cb = new ConfigurationBuilder().ApplyConventions(conventionContext, builder.Configuration);
        if (cb.Sources is { Count: > 0 })
        {
            configurationBuilder.Add(
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true
                }
            );
        }

        builder.ConfigureContainer(ConventionServiceProviderFactory.Wrap(conventionContext));
        return Task.FromResult(builder);
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

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="builderAction"></param>
    /// <returns>WebAssemblyHostBuilder.</returns>
    public static async Task<WebAssemblyHostBuilder> ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder,
        Action<ConventionContextBuilder> builderAction
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>());
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
    public static Task<WebAssemblyHostBuilder> ConfigureRocketSurgery(
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
            builder, a =>
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
    public static Task<WebAssemblyHostBuilder> ConfigureRocketSurgery(
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
    public static Task<WebAssemblyHostBuilder> ConfigureRocketSurgery(
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
            builder, a =>
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
}
