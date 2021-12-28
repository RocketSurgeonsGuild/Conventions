#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

#pragma warning disable CA1031
#pragma warning disable CA2000

namespace Rocket.Surgery.Web.Hosting;

/// <summary>
///     Class RocketHostExtensions.
/// </summary>
public static class RocketWebHostExtensions
{
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>WebApplicationBuilder.</returns>
    public static WebApplicationBuilder ConfigureRocketSurgery(this WebApplicationBuilder builder)
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
    /// <returns>WebApplicationBuilder.</returns>
    public static WebApplicationBuilder ConfigureRocketSurgery(this WebApplicationBuilder builder, Action<ConventionContextBuilder> action)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        builder.Host.Properties[typeof(WebApplicationBuilder)] = builder;
        action(SetupConventions(builder));
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <returns>IHostBuilder.</returns>
    public static WebApplicationBuilder ConfigureRocketSurgery(
        this WebApplicationBuilder builder, Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
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

        builder.Host.Properties[typeof(IHostBuilder)] = builder;
        SetupConventions(builder).WithConventionsFrom(getConventions);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <returns>WebApplicationBuilder.</returns>
    public static WebApplicationBuilder ConfigureRocketSurgery(this WebApplicationBuilder builder, ConventionContextBuilder conventionContextBuilder)
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
    /// <returns>WebApplicationBuilder.</returns>
    public static WebApplicationBuilder UseRocketBooster(
        this WebApplicationBuilder builder,
        Func<WebApplicationBuilder, ConventionContextBuilder> func,
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
    /// <returns>WebApplicationBuilder.</returns>
    public static WebApplicationBuilder LaunchWith(
        this WebApplicationBuilder builder,
        Func<WebApplicationBuilder, ConventionContextBuilder> func,
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
    internal static ConventionContextBuilder SetupConventions(WebApplicationBuilder builder)
    {
        if (builder.Host.Properties.ContainsKey(typeof(ConventionContextBuilder)))
            return ( builder.Host.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;

        var conventionContextBuilder = Configure(
            builder, new ConventionContextBuilder(builder.Host.Properties!).UseDependencyContext(DependencyContext.Default)
        );
        return conventionContextBuilder;
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder SetupConventions(WebApplicationBuilder builder, ConventionContextBuilder conventionContextBuilder)
    {
        if (builder.Host.Properties.ContainsKey(typeof(ConventionContextBuilder)))
            return ( builder.Host.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;

        return Configure(builder, conventionContextBuilder);
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder Configure(WebApplicationBuilder builder, ConventionContextBuilder contextBuilder)
    {
        HostingListener.Attach(builder, contextBuilder);
        builder.Host.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Host.Properties[typeof(WebApplicationBuilder)] = builder;
        return contextBuilder;
    }
}
#endif
