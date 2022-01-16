using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;

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
    public static WebAssemblyHostBuilder ConfigureRocketSurgery(this WebAssemblyHostBuilder builder, IConventionContext conventionContext)
    {
        new WrappedWebAssemblyHostBuilder(builder).ConfigureRocketSurgery(conventionContext);
        return builder;
    }

    /// <summary>
    ///     Apply the conventions to the builder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionContext"></param>
    public static IWebAssemblyHostBuilder ConfigureRocketSurgery(this IWebAssemblyHostBuilder builder, IConventionContext conventionContext)
    {
        conventionContext.Properties.AddIfMissing<IConfiguration>(builder.Configuration);
        conventionContext.Properties.AddIfMissing(builder.HostEnvironment);
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

        builder.Configuration.ApplyConventions(conventionContext, builder.Configuration);
        builder.ConfigureContainer(ConventionServiceProviderFactory.Wrap(conventionContext));
        return builder;
    }

    /// <summary>
    ///     Applys all conventions for hosting, configuration, services and logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="contextBuilder"></param>
    internal static IConventionContext ApplyConventions(WebAssemblyHostBuilder builder, ConventionContextBuilder contextBuilder)
    {
        return ApplyConventions(new WrappedWebAssemblyHostBuilder(builder), contextBuilder);
    }

    /// <summary>
    ///     Applys all conventions for hosting, configuration, services and logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="contextBuilder"></param>
    internal static IConventionContext ApplyConventions(IWebAssemblyHostBuilder builder, ConventionContextBuilder contextBuilder)
    {
        var context = ConventionContext.From(contextBuilder);
        builder.ConfigureRocketSurgery(context);
        return context;
    }

    /// <summary>
    ///     Applys all conventions for hosting, configuration, services and logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    internal static IConventionContext ApplyConventions(WebAssemblyHostBuilder builder, IConventionContext context)
    {
        return ApplyConventions(new WrappedWebAssemblyHostBuilder(builder), context);
    }

    /// <summary>
    ///     Applys all conventions for hosting, configuration, services and logging
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    internal static IConventionContext ApplyConventions(IWebAssemblyHostBuilder builder, IConventionContext context)
    {
        builder.ConfigureRocketSurgery(context);
        return context;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="builderAction"></param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static IWebAssemblyHostBuilder ConfigureRocketSurgery(this IWebAssemblyHostBuilder builder, Action<ConventionContextBuilder> builderAction)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>());
        builderAction.Invoke(contextBuilder);
        ApplyConventions(builder, contextBuilder);

        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="builderAction"></param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static WebAssemblyHostBuilder ConfigureRocketSurgery(this WebAssemblyHostBuilder builder, Action<ConventionContextBuilder> builderAction)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        ConfigureRocketSurgery(new WrappedWebAssemblyHostBuilder(builder), builderAction);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="appDomain"></param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static IWebAssemblyHostBuilder ConfigureRocketSurgery(
        this IWebAssemblyHostBuilder builder, AppDomain appDomain,
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
            }
        );
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="appDomain"></param>
    /// <param name="action"></param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static WebAssemblyHostBuilder ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder, AppDomain appDomain,
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
            }
        );
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies"></param>
    /// <param name="action"></param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static IWebAssemblyHostBuilder ConfigureRocketSurgery(
        this IWebAssemblyHostBuilder builder, IEnumerable<Assembly> assemblies,
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
            }
        );
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assemblies"></param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static WebAssemblyHostBuilder ConfigureRocketSurgery(
        this WebAssemblyHostBuilder builder, IEnumerable<Assembly> assemblies,
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
            }
        );
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static IWebAssemblyHostBuilder UseRocketBooster(
        this IWebAssemblyHostBuilder builder,
        Func<IWebAssemblyHostBuilder, ConventionContextBuilder> func,
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
        ApplyConventions(builder, b);
        action?.Invoke(b);
        return builder;
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static WebAssemblyHostBuilder UseRocketBooster(
        this WebAssemblyHostBuilder builder,
        Func<IWebAssemblyHostBuilder, ConventionContextBuilder> func,
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

        var innerBuilder = new WrappedWebAssemblyHostBuilder(builder);
        var b = func(innerBuilder);
        ApplyConventions(builder, b);
        action?.Invoke(b);
        return builder;
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static IWebAssemblyHostBuilder LaunchWith(
        this IWebAssemblyHostBuilder builder,
        Func<IWebAssemblyHostBuilder, ConventionContextBuilder> func,
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
        ApplyConventions(builder, b);
        action?.Invoke(b);
        return builder;
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IWebAssemblyHostBuilder.</returns>
    public static WebAssemblyHostBuilder LaunchWith(
        this WebAssemblyHostBuilder builder,
        Func<IWebAssemblyHostBuilder, ConventionContextBuilder> func,
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

        var innerBuilder = new WrappedWebAssemblyHostBuilder(builder);
        var b = func(innerBuilder);
        ApplyConventions(builder, b);
        action?.Invoke(b);
        return builder;
    }
}
