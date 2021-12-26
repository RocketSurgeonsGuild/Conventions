using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;

#pragma warning disable CA1031
#pragma warning disable CA2000

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketHostExtensions.
/// </summary>
public static class RocketHostExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder ConfigureRocketSurgery(this IHostBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return ConfigureRocketSurgery(builder, _ => { });
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="action">The action.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder ConfigureRocketSurgery(this IHostBuilder builder, Action<ConventionContextBuilder> action)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        builder.Properties[typeof(IHostBuilder)] = builder;
        action(SetupConventions(builder));
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder ConfigureRocketSurgery(this IHostBuilder builder, ConventionContextBuilder conventionContextBuilder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (conventionContextBuilder == null)
        {
            throw new ArgumentNullException(nameof(conventionContextBuilder));
        }

        builder.Properties[typeof(IHostBuilder)] = builder;
        SetupConventions(builder, conventionContextBuilder);
        return builder;
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder UseRocketBooster(
        this IHostBuilder builder,
        Func<IHostBuilder, ConventionContextBuilder> func,
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
        SetupConventions(builder, b);
        action?.Invoke(b);
        return builder;
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder LaunchWith(
        this IHostBuilder builder,
        Func<IHostBuilder, ConventionContextBuilder> func,
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
        SetupConventions(builder, b);
        action?.Invoke(b);
        return builder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>RocketHostBuilder.</returns>
    public static ConventionContextBuilder SetupConventions(IHostBuilder builder)
    {
        if (builder.Properties.ContainsKey(typeof(ConventionContextBuilder)))
            return ( builder.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;

        var conventionContextBuilder = Configure(builder, new ConventionContextBuilder(builder.Properties).UseDependencyContext(DependencyContext.Default));
        builder.Properties[typeof(ConventionContextBuilder)] = conventionContextBuilder;
        // builder.Properties[typeof(IHostBuilder)] = builder;
        return conventionContextBuilder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <returns>RocketHostBuilder.</returns>
    public static ConventionContextBuilder SetupConventions(IHostBuilder builder, ConventionContextBuilder conventionContextBuilder)
    {
        if (builder.Properties.ContainsKey(typeof(ConventionContextBuilder)))
            return ( builder.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;

        builder.Properties[typeof(ConventionContextBuilder)] = conventionContextBuilder;
        Configure(builder, conventionContextBuilder.UseDependencyContext(DependencyContext.Default));
        // builder.Properties[typeof(IHostBuilder)] = builder;
        return conventionContextBuilder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <returns>RocketHostBuilder.</returns>
    public static ConventionContextBuilder Configure(IHostBuilder builder, ConventionContextBuilder contextBuilder)
    {
        var host = new RocketContext(builder, contextBuilder);
        builder
           .ConfigureHostConfiguration(host.ComposeHostingConvention)
           .ConfigureHostConfiguration(host.CaptureArguments)
           .ConfigureAppConfiguration(host.ConfigureAppConfiguration)
           .ConfigureServices(host.ConfigureServices);
        builder.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        // builder.Properties[typeof(IHostBuilder)] = builder;
        return contextBuilder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="context"></param>
    /// <returns>RocketHostBuilder.</returns>
    public static IConventionContext Configure(IHostBuilder builder, IConventionContext context)
    {
        context.Properties.AddIfMissing(builder).AddIfMissing(HostType.Live);
        var host = new RocketContext(builder, context);
        builder
           .ConfigureHostConfiguration(host.ComposeHostingConvention)
           .ConfigureHostConfiguration(host.CaptureArguments)
           .ConfigureAppConfiguration(host.ConfigureAppConfiguration)
           .ConfigureServices(host.ConfigureServices);

        return context;
    }
}
