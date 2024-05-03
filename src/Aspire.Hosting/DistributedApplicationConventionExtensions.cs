using Aspire.Hosting;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Aspire.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static class DistributedApplicationConventionExtensions
{
    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedApplication(this ConventionContextBuilder container, DistributedApplicationConvention @delegate, int priority = 0)
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
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedApplication(this ConventionContextBuilder container, DistributedApplicationAsyncConvention @delegate, int priority = 0)
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
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedApplication(this ConventionContextBuilder container, Action<IDistributedApplicationBuilder> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationConvention((_, builder) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedApplication(this ConventionContextBuilder container, Func<IDistributedApplicationBuilder, ValueTask> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationAsyncConvention((_, builder, _) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedApplication(
        this ConventionContextBuilder container,
        Func<IDistributedApplicationBuilder, CancellationToken, ValueTask> @delegate, int priority = 0
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationAsyncConvention((_, builder, cancellationToken) => @delegate(builder, cancellationToken)));
        return container;
    }
}
