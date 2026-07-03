using Clavus.Hosting;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Clavus;

/// <summary>
///     Helper method for working with <see cref="ClavusContextBuilder" />
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
    public static ClavusContextBuilder ConfigureApplication<TBuilder>(
        this ClavusContextBuilder container,
        HostApplicationPart<TBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
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
    public static ClavusContextBuilder ConfigureApplication<TBuilder>(
        this ClavusContextBuilder container,
        HostApplicationAsyncPart<TBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
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
    public static ClavusContextBuilder ConfigureApplication<TBuilder>(
        this ClavusContextBuilder container,
        Action<TBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationPart<TBuilder>((_, builder) => @delegate(builder)), priority, category);
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
    public static ClavusContextBuilder ConfigureApplication<TBuilder>(
        this ClavusContextBuilder container,
        Func<IHostApplicationBuilder, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationAsyncPart<TBuilder>((_, builder, _) => @delegate(builder)), priority, category);
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
    public static ClavusContextBuilder ConfigureApplication<TBuilder>(
        this ClavusContextBuilder container,
        Func<IHostApplicationBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new HostApplicationAsyncPart<TBuilder>((_, builder, cancellationToken) => @delegate(builder, cancellationToken)),
            priority,
            category
        );
        return container;
    }
}
