using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;
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

        var contextBuilder = GetOrCreate(
            builder, () =>
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
#pragma warning disable RCS1249
                new ConventionContextBuilder(builder.Host.Properties!)
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

        var contextBuilder = GetOrCreate(
            builder, () =>
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                new ConventionContextBuilder(builder.Host.Properties)
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

        Configure(builder, conventionContextBuilder);
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
    private static ConventionContextBuilder GetOrCreate(WebApplicationBuilder builder, Func<ConventionContextBuilder> factory)
    {
        return builder.Host.Properties.TryGetValue(typeof(ConventionContextBuilder), out var value) && value is ConventionContextBuilder cb ? cb : factory();
    }

    /// <summary>
    ///     Gets the or create builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="contextBuilder"></param>
    /// <returns>RocketHostBuilder.</returns>
    internal static ConventionContextBuilder Configure(WebApplicationBuilder builder, ConventionContextBuilder contextBuilder)
    {
        contextBuilder.Properties
                      .AddIfMissing(builder)
                      .AddIfMissing(contextBuilder)
                      .AddIfMissing(HostType.Live);
        builder.Host.Properties[typeof(ConventionContextBuilder)] = contextBuilder;
        builder.Host.Properties[typeof(WebApplicationBuilder)] = builder;

        if (contextBuilder.Properties.ContainsKey(typeof(RocketWebHostExtensions))) return contextBuilder;
        contextBuilder.Properties.Add(typeof(RocketWebHostExtensions), true);
        builder.Host.UseServiceProviderFactory(
            _ => LazyConventionServiceProviderFactory.Create(
                () =>
                {
                    var host = new RocketContext(builder, ConventionContext.From(contextBuilder));
                    host.ComposeHostingConvention();
                    host.ConfigureAppConfiguration();
                    host.ConfigureServices();
                    return host.UseServiceProviderFactory();
                }
            )
        );
        return contextBuilder;
    }
}
