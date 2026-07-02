using Aspire.Hosting.Testing;
using Rocket.Surgery.Clavus.Aspire.Testing;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Clavus;

/// <summary>
///     Helper method for working with <see cref="ClavusContextBuilder" />
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
    /// <param name="category">The category.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDistributedTestingApplication(
        this ClavusContextBuilder container,
        DistributedApplicationTestingConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
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
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDistributedTestingApplication(
        this ClavusContextBuilder container,
        DistributedApplicationTestingAsyncConvention @delegate,
        int priority = 0,
        ClavusCategory? category = null
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
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder ConfigureDistributedTestingApplication(
        this ClavusContextBuilder container,
        Action<IDistributedApplicationTestingBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationTestingConvention((_, builder) => @delegate(builder)), priority, category);
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
    public static ClavusContextBuilder ConfigureDistributedTestingApplication(
        this ClavusContextBuilder container,
        Func<IDistributedApplicationTestingBuilder, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new DistributedApplicationTestingAsyncConvention((_, builder, _) => @delegate(builder)), priority, category);
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
    public static ClavusContextBuilder ConfigureDistributedTestingApplication(
        this ClavusContextBuilder container,
        Func<IDistributedApplicationTestingBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new DistributedApplicationTestingAsyncConvention((_, builder, cancellationToken) => @delegate(builder, cancellationToken)),
            priority,
            category
        );
        return container;
    }
}
