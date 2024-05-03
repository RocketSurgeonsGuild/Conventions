using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Aspire.Hosting;
using Rocket.Surgery.Aspire.Hosting.Testing;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static class DistributedApplicationTestingConventionExtensions
{
    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedTestingApplication(this ConventionContextBuilder container, DistributedApplicationTestingConvention @delegate, int priority = 0)
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
    public static ConventionContextBuilder ConfigureDistributedTestingApplication(this ConventionContextBuilder container, DistributedApplicationTestingAsyncConvention @delegate, int priority = 0)
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
    public static ConventionContextBuilder ConfigureDistributedTestingApplication(this ConventionContextBuilder container, Action<IDistributedApplicationTestingBuilder> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationTestingConvention((_, builder) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedTestingApplication(this ConventionContextBuilder container, Func<IDistributedApplicationTestingBuilder, ValueTask> @delegate, int priority = 0)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationTestingAsyncConvention((_, builder, _) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureDistributedTestingApplication(
        this ConventionContextBuilder container,
        Func<IDistributedApplicationTestingBuilder, CancellationToken, ValueTask> @delegate, int priority = 0
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationTestingAsyncConvention((_, builder, cancellationToken) => @delegate(builder, cancellationToken)));
        return container;
    }
}
