using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Autofac;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Class AutofacRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class AutofacConventionRocketHostExtensions
{
    /// <summary>
    ///     Uses Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder UseAutofac(this ConventionContextBuilder builder, ContainerBuilder? containerBuilder = null)
    {
        return builder.UseServiceProviderFactory<ContainerBuilder>(
            async (context, services, ct) =>
            {
                var c = containerBuilder ?? new ContainerBuilder();
                await c.ApplyConventionsAsync(context, services, ct);
                return new AutofacConventionServiceProviderFactory(c);
            }
        );
    }

    /// <summary>
    ///     Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(
        this ConventionContextBuilder builder,
        AutofacConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(@delegate, priority, category ?? ConventionCategory.Core);
        return builder;
    }

    /// <summary>
    ///     Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(
        this ConventionContextBuilder builder,
        Action<IConfiguration, IServiceCollection, ContainerBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(
            new AutofacConvention((_, configuration, services, container) => @delegate(configuration, services, container)),
            priority,
            category ?? ConventionCategory.Core
        );
        return builder;
    }

    /// <summary>
    ///     Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(
        this ConventionContextBuilder builder,
        Action<IServiceCollection, ContainerBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(new AutofacConvention((_, _, services, container) => @delegate(services, container)), priority, category ?? ConventionCategory.Core);
        return builder;
    }

    /// <summary>
    ///     Uses the Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureAutofac(
        this ConventionContextBuilder builder,
        Action<ContainerBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AppendDelegate(new AutofacConvention((_, _, _, container) => @delegate(container)), priority, category ?? ConventionCategory.Core);
        return builder;
    }
}
