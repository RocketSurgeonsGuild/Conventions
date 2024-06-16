//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Generated_HostApplicationBuilder_Extensions.g.cs
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8603, CS8602, CS8618
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.Extensions.Hosting.HostApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

namespace Rocket.Surgery.Hosting;

internal static partial class GeneratedRocketHostApplicationBuilderExtensions
{
    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(func);
        var b = await func(builder, cancellationToken);
        return await ConfigureRocketSurgery(builder, b, cancellationToken);
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, cancellationToken);

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(action);
        var b = await func(builder, cancellationToken);
        await action(b, cancellationToken);
        return await ConfigureRocketSurgery(builder, b, cancellationToken);
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, action, cancellationToken);

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(action);
        var b = await func(builder, cancellationToken);
        await action(b);
        return await ConfigureRocketSurgery(builder, b, cancellationToken);
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, action, cancellationToken);

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(action);
        var b = await func(builder, cancellationToken);
        action(b);
        return await ConfigureRocketSurgery(builder, b, cancellationToken);
    }
    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> UseRocketBooster(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, action, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, action, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, action, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, action, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, action, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, AppDelegate func, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => UseRocketBooster(builder, func, action, cancellationToken);

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> LaunchWith(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, AppDelegate func, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => await UseRocketBooster(await builder, func, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder);
        action(contextBuilder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="action">The configuration action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, IConventionFactory getConventions, Func<ConventionContextBuilder, CancellationToken, ValueTask> action ,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(getConventions);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder).UseConventionFactory(getConventions);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="action">The configuration action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, IConventionFactory getConventions, Func<ConventionContextBuilder, CancellationToken, ValueTask> action ,CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, getConventions, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="action">The configuration action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder,IConventionFactory getConventions, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(getConventions);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder).UseConventionFactory(getConventions);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="action">The configuration action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder,IConventionFactory getConventions, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, getConventions, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, IConventionFactory getConventions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(getConventions);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder).UseConventionFactory(getConventions);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, IConventionFactory getConventions, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, getConventions, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="action">The configuration action.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, IConventionFactory getConventions, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(getConventions);
        ArgumentNullException.ThrowIfNull(action);
        var contextBuilder = Rocket.Surgery.Hosting.RocketHostApplicationExtensions.GetExisting(builder).UseConventionFactory(getConventions);
        action(contextBuilder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <param name="action">The configuration action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, IConventionFactory getConventions, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, getConventions, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ConventionContextBuilder contextBuilder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        ArgumentNullException.ThrowIfNull(action);
        await action(contextBuilder, cancellationToken);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ConventionContextBuilder contextBuilder, Func<ConventionContextBuilder, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ConventionContextBuilder contextBuilder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        ArgumentNullException.ThrowIfNull(action);
        await action(contextBuilder);
        return await ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ConventionContextBuilder contextBuilder, Func<ConventionContextBuilder, ValueTask> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ConventionContextBuilder contextBuilder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        ArgumentNullException.ThrowIfNull(action);
        action(contextBuilder);
        return ConfigureRocketSurgery(builder, contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ConventionContextBuilder contextBuilder, Action<ConventionContextBuilder> action, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, action, cancellationToken);

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Microsoft.Extensions.Hosting.HostApplicationBuilder builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);
        return await Rocket.Surgery.Hosting.RocketHostApplicationExtensions.Configure(builder, static b => b.Build(), contextBuilder, cancellationToken);
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder">The convention context builder.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<Microsoft.Extensions.Hosting.IHost> ConfigureRocketSurgery(this Task<Microsoft.Extensions.Hosting.HostApplicationBuilder> builder, ConventionContextBuilder contextBuilder, CancellationToken cancellationToken = default) => await ConfigureRocketSurgery(await builder, contextBuilder, cancellationToken);
}