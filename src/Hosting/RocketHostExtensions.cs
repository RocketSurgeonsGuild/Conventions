using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
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

        ;
        action(SetupConventions(builder));
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder ConfigureRocketSurgery(
        this IHostBuilder builder, Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (getConventions == null)
        {
            throw new ArgumentNullException(nameof(getConventions));
        }

        SetupConventions(builder).WithConventionsFrom(getConventions);
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
    internal static ConventionContextBuilder SetupConventions(IHostBuilder builder)
    {
        if (builder.Properties.ContainsKey(typeof(ConventionContextBuilder)))
            return ( builder.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;

        var conventionContextBuilder = Configure(builder, new ConventionContextBuilder(builder.Properties).UseDependencyContext(DependencyContext.Default));
        return conventionContextBuilder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder SetupConventions(IHostBuilder builder, ConventionContextBuilder conventionContextBuilder)
    {
        if (builder.Properties.ContainsKey(typeof(ConventionContextBuilder)))
            return ( builder.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;

        Configure(builder, conventionContextBuilder);
        return conventionContextBuilder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder Configure(IHostBuilder builder, ConventionContextBuilder contextBuilder)
    {
        contextBuilder.Properties.AddIfMissing(builder).AddIfMissing(HostType.Live);
        builder.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Properties[typeof(IHostBuilder)] = builder;
        var host = new RocketContext(builder);
        builder
           .ConfigureHostConfiguration(host.ComposeHostingConvention)
           .ConfigureAppConfiguration(host.ConfigureAppConfiguration)
           .ConfigureServices(host.ConfigureServices)
           .UseServiceProviderFactory(host.UseServiceProviderFactory);
        return contextBuilder;
    }
}
