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
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, HostApplicationConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, HostApplicationAsyncConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, Action<IHostApplicationBuilder> @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationConvention((_, builder) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(this ConventionContextBuilder container, Func<IHostApplicationBuilder, ValueTask> @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationAsyncConvention((_, builder, _) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureApplication(
        this ConventionContextBuilder container,
        Func<IHostApplicationBuilder, CancellationToken, ValueTask> @delegate
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostApplicationAsyncConvention((_, builder, cancellationToken) => @delegate(builder, cancellationToken)));
        return container;
    }
}
