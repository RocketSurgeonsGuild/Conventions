#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.Extensions.Hosting.IHostApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ConventionsDelegate =
    System.Func<System.IServiceProvider, System.Collections.Generic.IEnumerable<Rocket.Surgery.Conventions.IConventionWithDependencies>>;

#pragma warning disable CA1031
#pragma warning disable CA2000
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketHostExtensions.
/// </summary>
[PublicAPI]
public static class RocketHostApplicationExtensions
{
    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<T> UseRocketBooster<T>(
        this T builder,
        AppDelegate func,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<T> UseRocketBooster<T>(
        this T builder,
        AppDelegate func,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    public static ValueTask<T> UseRocketBooster<T>(
        this T builder,
        AppDelegate func,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    public static ValueTask<T> UseRocketBooster<T>(
        this T builder,
        AppDelegate func,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    public static ValueTask<T> LaunchWith<T>(
        this T builder,
        AppDelegate func,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    public static ValueTask<T> LaunchWith<T>(
        this T builder,
        AppDelegate func,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    public static ValueTask<T> LaunchWith<T>(
        this T builder,
        AppDelegate func,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    public static ValueTask<T> LaunchWith<T>(this T builder, AppDelegate func, CancellationToken cancellationToken) where T : IHostApplicationBuilder
    {
        return UseRocketBooster(builder, func, cancellationToken);
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<T> LaunchWith<T>(this T builder, AppDelegate func) where T : IHostApplicationBuilder
    {
        return UseRocketBooster(builder, func, CancellationToken.None);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static ValueTask<T> ConfigureRocketSurgery<T>(
        this T builder,
        CancellationToken cancellationToken =
            default
    ) where T : IHostApplicationBuilder
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
    public static async ValueTask<T> ConfigureRocketSurgery<T>(
        this T builder,
        Action<ConventionContextBuilder> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var contextBuilder = new ConventionContextBuilder(builder.Properties!).UseDependencyContext(DependencyContext.Default!);
        await Configure(builder, contextBuilder, cancellationToken);
        action(contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<T> ConfigureRocketSurgery<T>(
        this T builder,
        Func<ConventionContextBuilder, ValueTask> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var contextBuilder = new ConventionContextBuilder(builder.Properties!).UseDependencyContext(DependencyContext.Default!);
        await Configure(builder, contextBuilder, cancellationToken);
        await action(contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<T> ConfigureRocketSurgery<T>(
        this T builder,
        Func<ConventionContextBuilder, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        var contextBuilder = new ConventionContextBuilder(builder.Properties!).UseDependencyContext(DependencyContext.Default!);
        await Configure(builder, contextBuilder, cancellationToken);
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
    public static async ValueTask<T> ConfigureRocketSurgery<T>(
        this T builder,
        ConventionsDelegate getConventions,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
    {
        var contextBuilder = new ConventionContextBuilder(builder.Properties!)
                             #pragma warning restore RCS1249
                             // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                            .UseDependencyContext(DependencyContext.Default!)
                            .WithConventionsFrom(getConventions);
        await Configure(builder, contextBuilder, cancellationToken);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static async ValueTask<T> ConfigureRocketSurgery<T>(
        this T builder,
        ConventionContextBuilder conventionContextBuilder,
        CancellationToken cancellationToken = default
    ) where T : IHostApplicationBuilder
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
    ///     Method used to get an existing <see cref="ConventionContextBuilder" /> or create and insert a new one.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="factory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static ValueTask<ConventionContextBuilder> GetOrCreate(
        IHostApplicationBuilder builder,
        Func<CancellationToken, ValueTask<ConventionContextBuilder>> factory,
        CancellationToken cancellationToken
    )
    {
        return builder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var value)
            ? ValueTask.FromResult(value as ConventionContextBuilder)!
            : factory(cancellationToken);
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static async ValueTask<ConventionContextBuilder> Configure(
        IHostApplicationBuilder builder,
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
        builder.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Properties[typeof(IHostApplicationBuilder)] = builder;
        builder.Properties[builder.GetType()] = builder;

        if (contextBuilder.Properties.ContainsKey(typeof(RocketHostApplicationExtensions))) return contextBuilder;

        var host = new RocketApplicationBuilderContext(builder, await ConventionContext.FromAsync(contextBuilder, cancellationToken));
        await host.ComposeHostingConvention(cancellationToken);
        await host.ConfigureAppConfiguration(cancellationToken);
        await host.ConfigureServices(cancellationToken);

        contextBuilder.Properties.Add(typeof(RocketHostApplicationExtensions), true);
        builder.ConfigureContainer(host.UseServiceProviderFactory());
        return contextBuilder;
    }
}
#endif