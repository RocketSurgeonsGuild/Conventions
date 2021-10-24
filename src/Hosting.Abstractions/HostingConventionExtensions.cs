using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
public static class HostingConventionExtensions
{
    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureHosting(this ConventionContextBuilder container, HostingConvention @delegate)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the hosting delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureHosting(this ConventionContextBuilder container, Action<IHostBuilder> @delegate)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        container.AppendDelegate(new HostingConvention((_, builder) => @delegate(builder)));
        return container;
    }
}
