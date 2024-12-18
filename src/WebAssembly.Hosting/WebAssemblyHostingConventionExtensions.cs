using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.WebAssembly.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static class WebAssemblyHostingConventionExtensions
{
    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>ConventionContextBuilder.</returns>
    public static ConventionContextBuilder ConfigureWebAssembly(
        this ConventionContextBuilder container,
        WebAssemblyHostingConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
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
    /// <returns>ConventionContextBuilder.</returns>
    public static ConventionContextBuilder ConfigureWebAssembly(
        this ConventionContextBuilder container,
        WebAssemblyHostingAsyncConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
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
    /// <returns>ConventionContextBuilder.</returns>
    public static ConventionContextBuilder ConfigureWebAssembly(
        this ConventionContextBuilder container,
        Action<WebAssemblyHostBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new WebAssemblyHostingConvention((_, builder) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>ConventionContextBuilder.</returns>
    public static ConventionContextBuilder ConfigureWebAssembly(
        this ConventionContextBuilder container,
        Func<WebAssemblyHostBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new WebAssemblyHostingAsyncConvention((_, builder, _) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns>ConventionContextBuilder.</returns>
    public static ConventionContextBuilder ConfigureWebAssembly(
        this ConventionContextBuilder container,
        Func<WebAssemblyHostBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new WebAssemblyHostingAsyncConvention((_, builder, cancellationToken) => @delegate(builder, cancellationToken)),
            priority,
            category
        );
        return container;
    }
}
