using System.Reflection;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting;

/// <summary>
///     Class RocketBooster.
/// </summary>
public static class RocketBooster
{
    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> ForAppDomain(AppDomain appDomain)
    {
        return builder => new ConventionContextBuilder(new Dictionary<object, object?>()).UseAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> ForAppDomain(
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return builder => ForAppDomain(appDomain)(builder).WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> For(AppDomain appDomain)
    {
        return ForAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> For(
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAppDomain(appDomain, getConventions);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> ForAssemblies(IEnumerable<Assembly> assemblies)
    {
        return builder => new ConventionContextBuilder(new Dictionary<object, object?>()).UseAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> ForAssemblies(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return builder => ForAssemblies(assemblies)(builder).WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> For(IEnumerable<Assembly> assemblies)
    {
        return ForAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<IWebAssemblyHostBuilder, ConventionContextBuilder> For(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAssemblies(assemblies, getConventions);
    }
}
