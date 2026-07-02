using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus.DryIoc;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Clavus;

/// <summary>
///     Class DryIocRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class DryIocConventionRocketHostExtensions
{
    /// <summary>
    ///     Uses DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="container">The container.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder UseDryIoc(this ClavusContextBuilder builder, IContainer? container = null)
    {
        return builder.UseServiceProviderFactory<IContainer>(
            async (context, services, ct) =>
            {
                var c = ( container ?? new Container() ).With(r => r.WithBaseMicrosoftDependencyInjectionRules(null));
                return new DryIocConventionServiceProviderFactory(context, await c.ApplyConventionsAsync(context, services, ct));
            }
        );
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        DryIocConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Action<IClavusContext, IConfiguration, IServiceCollection, IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new DryIocConvention(
                (context, configuration, services, container) =>
                {
                    @delegate(context, configuration, services, container);
                    return container;
                }
            ),
            priority,
            category ?? ClavusCategory.Core
        );
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Func<IServiceCollection, IContainer, IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(new DryIocConvention((_, _, services, container) => @delegate(services, container)), priority, category);
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Action<IConfiguration, IServiceCollection, IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new DryIocConvention(
                (_, configuration, services, container) =>
                {
                    @delegate(configuration, services, container);
                    return container;
                }
            ),
            priority,
            category ?? ClavusCategory.Core
        );
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Func<IConfiguration, IServiceCollection, IContainer, IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new DryIocConvention((_, configuration, services, container) => @delegate(configuration, services, container)),
            priority,
            category ?? ClavusCategory.Core
        );
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Action<IServiceCollection, IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new DryIocConvention(
                (_, _, services, container) =>
                {
                    @delegate(services, container);
                    return container;
                }
            ),
            priority,
            category ?? ClavusCategory.Core
        );
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Func<IContainer, IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(new DryIocConvention((_, _, _, container) => @delegate(container)), priority, category ?? ClavusCategory.Core);
        return builder;
    }

    /// <summary>
    ///     Uses the DryIoc.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDryIoc(
        this ClavusContextBuilder builder,
        Action<IContainer> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new DryIocConvention(
                (_, _, _, container) =>
                {
                    @delegate(container);
                    return container;
                }
            ),
            priority,
            category ?? ClavusCategory.Core
        );
        return builder;
    }
}
