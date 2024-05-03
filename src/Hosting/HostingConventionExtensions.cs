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
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, HostApplicationConvention @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, HostApplicationAsyncConvention @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate, priority);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, Action<IHostApplicationBuilder> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationConvention((_, builder) => @delegate(builder)), priority);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, Func<IHostApplicationBuilder, ValueTask> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationAsyncConvention((_, builder, _) => @delegate(builder)), priority);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(
        this ConventionContextBuilder container,
        Func<IHostApplicationBuilder, CancellationToken, ValueTask> @delegate, int priority = 0
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationAsyncConvention((_, builder, cancellationToken) => @delegate(builder, cancellationToken)), priority);
        return container;
    }
}
