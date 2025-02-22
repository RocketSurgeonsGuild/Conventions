using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.DryIoc;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

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
    public static ConventionContextBuilder UseDryIoc(this ConventionContextBuilder builder, IContainer? container = null)
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        DryIocConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(@delegate, priority, category ?? ConventionCategory.Core);
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Action<IConventionContext, IConfiguration, IServiceCollection, IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
            category ?? ConventionCategory.Core
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Func<IServiceCollection, IContainer, IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Action<IConfiguration, IServiceCollection, IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
            category ?? ConventionCategory.Core
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Func<IConfiguration, IServiceCollection, IContainer, IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new DryIocConvention((_, configuration, services, container) => @delegate(configuration, services, container)),
            priority,
            category ?? ConventionCategory.Core
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Action<IServiceCollection, IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
            category ?? ConventionCategory.Core
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Func<IContainer, IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(new DryIocConvention((_, _, _, container) => @delegate(container)), priority, category ?? ConventionCategory.Core);
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
    public static ConventionContextBuilder ConfigureDryIoc(
        this ConventionContextBuilder builder,
        Action<IContainer> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
            category ?? ConventionCategory.Core
        );
        return builder;
    }
}
