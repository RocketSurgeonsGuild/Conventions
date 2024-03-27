using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

#pragma warning disable CA1031
#pragma warning disable CA2000
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

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

        var contextBuilder = GetOrCreate(
            builder,
            () => new ConventionContextBuilder(builder.Properties)
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
               .UseDependencyContext(DependencyContext.Default!)
        );
        action(contextBuilder);
        Configure(builder, contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder ConfigureRocketSurgery(
        this IHostBuilder builder,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
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

        var contextBuilder = GetOrCreate(
            builder,
            () => new ConventionContextBuilder(builder.Properties)
                  // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                 .UseDependencyContext(DependencyContext.Default!)
                 .WithConventionsFrom(getConventions)
        );
        Configure(builder, contextBuilder);
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

        Configure(builder, conventionContextBuilder);
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

        var b = GetOrCreate(builder, () => func(builder));
        action?.Invoke(b);
        Configure(builder, b);
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

        var b = GetOrCreate(builder, () => func(builder));
        action?.Invoke(b);
        Configure(builder, b);
        return builder;
    }

    /// <summary>
    ///     Method used to get an existing <see cref="ConventionContextBuilder" /> or create and insert a new one.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    internal static ConventionContextBuilder GetOrCreate(IHostBuilder builder, Func<ConventionContextBuilder> factory)
    {
        return builder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var value) ? ( value as ConventionContextBuilder )! : factory();
    }

    #if NET8_0_OR_GREATER
    /// <summary>
    ///     Method used to get an existing <see cref="ConventionContextBuilder" /> or create and insert a new one.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    internal static ConventionContextBuilder GetOrCreate(IHostApplicationBuilder builder, Func<ConventionContextBuilder> factory)
    {
        return builder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var value) ? ( value as ConventionContextBuilder )! : factory();
    }
    #endif

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder Configure(IHostBuilder builder, ConventionContextBuilder contextBuilder)
    {
        contextBuilder
           .Properties
           .AddIfMissing(builder)
           .AddIfMissing(contextBuilder)
           .AddIfMissing(HostType.Live);
        builder.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Properties[typeof(IHostBuilder)] = builder;

        if (contextBuilder.Properties.ContainsKey(typeof(RocketHostExtensions))) return contextBuilder;
        contextBuilder.Properties.Add(typeof(RocketHostExtensions), true);
        var host = new RocketContext(builder);
        builder
           .ConfigureHostConfiguration(host.ComposeHostingConvention)
           .ConfigureAppConfiguration(host.ConfigureAppConfiguration)
           .ConfigureServices(host.ConfigureServices)
           .UseServiceProviderFactory(host.UseServiceProviderFactory);
        return contextBuilder;
    }

    #if NET8_0_OR_GREATER
    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder Configure(IHostApplicationBuilder builder, ConventionContextBuilder contextBuilder)
    {
        contextBuilder
           .Properties
           .AddIfMissing(builder)
           .AddIfMissing(contextBuilder)
           .AddIfMissing(HostType.Live);
        builder.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Properties[typeof(IHostApplicationBuilder)] = builder;

        if (contextBuilder.Properties.ContainsKey(typeof(RocketHostExtensions))) return contextBuilder;
        contextBuilder.Properties.Add(typeof(RocketHostExtensions), true);
        builder.ConfigureContainer(
            LazyConventionServiceProviderFactory.Create(
                () =>
                {
                    var host = new RocketApplicationBuilderContext(builder, ConventionContext.From(contextBuilder));
                    host.ComposeHostingConvention();
                    host.ConfigureAppConfiguration();
                    host.ConfigureServices();
                    return host.UseServiceProviderFactory();
                }
            )
        );
        return contextBuilder;
    }
    #endif
    #if NET8_0_OR_GREATER
    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static IHostApplicationBuilder ConfigureRocketSurgery(this IHostApplicationBuilder builder)
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
    /// <returns>IHostApplicationBuilder.</returns>
    public static IHostApplicationBuilder ConfigureRocketSurgery(this IHostApplicationBuilder builder, Action<ConventionContextBuilder> action)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var contextBuilder = GetOrCreate(
            builder,
            () =>
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                #pragma warning disable RCS1249
                new ConventionContextBuilder(builder.Properties!)
                    #pragma warning restore RCS1249
                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   .UseDependencyContext(DependencyContext.Default!)
        );
        action(contextBuilder);
        Configure(builder, contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="getConventions">The method to get the conventions.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostApplicationBuilder ConfigureRocketSurgery(
        this IHostApplicationBuilder builder,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
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

        var contextBuilder = GetOrCreate(
            builder,
            () =>
                #pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                new ConventionContextBuilder(builder.Properties)
                    #pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   .UseDependencyContext(DependencyContext.Default!)
                   .WithConventionsFrom(getConventions)
        );
        Configure(builder, contextBuilder);
        return builder;
    }

    /// <summary>
    ///     Configures the rocket Surgery.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="conventionContextBuilder">The convention context builder.</param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static IHostApplicationBuilder ConfigureRocketSurgery(this IHostApplicationBuilder builder, ConventionContextBuilder conventionContextBuilder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (conventionContextBuilder == null)
        {
            throw new ArgumentNullException(nameof(conventionContextBuilder));
        }

        Configure(builder, conventionContextBuilder);
        return builder;
    }

    /// <summary>
    ///     Uses the rocket booster.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static IHostApplicationBuilder UseRocketBooster(
        this IHostApplicationBuilder builder,
        Func<IHostApplicationBuilder, ConventionContextBuilder> func,
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

        var b = GetOrCreate(builder, () => func(builder));
        action?.Invoke(b);
        Configure(builder, b);
        return builder;
    }

    /// <summary>
    ///     Launches the with.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="func">The function.</param>
    /// <param name="action">The action.</param>
    /// <returns>IHostApplicationBuilder.</returns>
    public static IHostApplicationBuilder LaunchWith(
        this IHostApplicationBuilder builder,
        Func<IHostApplicationBuilder, ConventionContextBuilder> func,
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

        var b = GetOrCreate(builder, () => func(builder));
        action?.Invoke(b);
        Configure(builder, b);
        return builder;
    }
    #endif
}
