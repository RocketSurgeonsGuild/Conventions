using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static class HostingConventionExtensions
{
    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication<TBuilder>(
        this ConventionContextBuilder container,
        HostApplicationConvention<TBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication<TBuilder>(
        this ConventionContextBuilder container,
        HostApplicationAsyncConvention<TBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication<TBuilder>(
        this ConventionContextBuilder container,
        Action<TBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationConvention<TBuilder>((_, builder) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication<TBuilder>(
        this ConventionContextBuilder container,
        Func<IHostApplicationBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationAsyncConvention<TBuilder>((_, builder, _) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication<TBuilder>(
        this ConventionContextBuilder container,
        Func<IHostApplicationBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new HostApplicationAsyncConvention<TBuilder>((_, builder, cancellationToken) => @delegate(builder, cancellationToken)),
            priority,
            category
        );
        return container;
    }
}
