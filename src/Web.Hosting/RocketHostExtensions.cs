using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.AspNetCore.Builder.WebApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ConventionsDelegate =
    System.Func<System.IServiceProvider, System.Collections.Generic.IEnumerable<Rocket.Surgery.Conventions.IConventionWithDependencies>>;

#pragma warning disable CA1031
#pragma warning disable CA2000

namespace Rocket.Surgery.Web.Hosting;

/// <summary>
///     Class RocketHostExtensions.
/// </summary>
[PublicAPI]
public static class RocketWebHostExtensions
{
    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static async ValueTask<WebApplicationBuilder> UseRocketBooster(
        this WebApplicationBuilder builder,
        AppDelegate func,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
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

        var b = await GetOrCreate(builder, ct => func(builder, ct), cancellationToken);
        await Configure(builder, b, cancellationToken);
        await ( action?.Invoke(b, cancellationToken) ?? ValueTask.CompletedTask );
        return builder;
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> UseRocketBooster(
        this WebApplicationBuilder builder,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> UseRocketBooster(
        this WebApplicationBuilder builder,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> UseRocketBooster(
        this WebApplicationBuilder builder,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> LaunchWith(
        this WebApplicationBuilder builder,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> LaunchWith(
        this WebApplicationBuilder builder,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> LaunchWith(
        this WebApplicationBuilder builder,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> LaunchWith(this WebApplicationBuilder builder, AppDelegate func, CancellationToken cancellationToken)
    {
        return UseRocketBooster(builder, func, cancellationToken);
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> LaunchWith(this WebApplicationBuilder builder, AppDelegate func)
    {
        return UseRocketBooster(builder, func, CancellationToken.None);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static ValueTask<WebApplicationBuilder> ConfigureRocketSurgery(this WebApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        return ConfigureRocketSurgery(builder, _ => { }, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static async ValueTask<WebApplicationBuilder> ConfigureRocketSurgery(
        this WebApplicationBuilder builder,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var contextBuilder = await RocketBooster.For(DependencyContext.Default!)(builder, cancellationToken);
        action(contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static async ValueTask<WebApplicationBuilder> ConfigureRocketSurgery(
        this WebApplicationBuilder builder,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var contextBuilder = await RocketBooster.For(DependencyContext.Default!)(builder, cancellationToken);
        await action(contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static async ValueTask<WebApplicationBuilder> ConfigureRocketSurgery(
        this WebApplicationBuilder builder,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var contextBuilder = await RocketBooster.For(DependencyContext.Default!)(builder, cancellationToken);
        await action(contextBuilder, cancellationToken);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostBuilder.</returns>
    public static async ValueTask<WebApplicationBuilder> ConfigureRocketSurgery(
        this WebApplicationBuilder builder,
        ConventionsDelegate getConventions,
        CancellationToken cancellationToken = default
    )
    {
        await RocketBooster.For(getConventions)(builder, cancellationToken);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>WebApplicationBuilder.</returns>
    public static async ValueTask<WebApplicationBuilder> ConfigureRocketSurgery(
        this WebApplicationBuilder builder,
        ConventionContextBuilder conventionContextBuilder,
        CancellationToken cancellationToken = default
    )

    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (conventionContextBuilder == null)
        {
            throw new ArgumentNullException(nameof(conventionContextBuilder));
        }

        await Configure(builder, conventionContextBuilder, cancellationToken);
        return builder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static async ValueTask<ConventionContextBuilder> Configure(
        WebApplicationBuilder builder,
        ConventionContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
    {
        contextBuilder
           .Properties
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType().FullName!, builder)
           .AddIfMissing(contextBuilder)
           .AddIfMissing(HostType.Live);
        builder.Host.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Host.Properties[typeof(WebApplicationBuilder)] = builder;

        if (contextBuilder.Properties.ContainsKey(typeof(RocketWebHostExtensions))) return contextBuilder;
        var host = new RocketContext(builder, ConventionContext.From(contextBuilder));
        await host.ComposeHostingConvention(cancellationToken);
        await host.ConfigureAppConfiguration(cancellationToken);
        await host.ConfigureServices(cancellationToken);

        contextBuilder.Properties.Add(typeof(RocketWebHostExtensions), true);
        builder.Host.UseServiceProviderFactory(host.UseServiceProviderFactory());
        return contextBuilder;
    }

    /// <summary>
    ///     Method used to get an existing <see cref="ConventionContextBuilder" /> or create and insert a new one.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    private static ConventionContextBuilder GetOrCreate(WebApplicationBuilder builder, Func<ConventionContextBuilder> factory)
    {
        return builder.Host.Properties.TryGetValue(typeof(ConventionContextBuilder), out var value) && value is ConventionContextBuilder cb ? cb : factory();
    }

    /// <summary>
    ///     Method used to get an existing <see cref="ConventionContextBuilder" /> or create and insert a new one.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="factory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async ValueTask<ConventionContextBuilder> GetOrCreate(
        WebApplicationBuilder builder,
        Func<CancellationToken, ValueTask<ConventionContextBuilder>> factory,
        CancellationToken cancellationToken
    )
    {
        return builder.Host.Properties.TryGetValue(typeof(ConventionContextBuilder), out var value) && value is ConventionContextBuilder cb
            ? cb
            : await factory(cancellationToken);
    }
}