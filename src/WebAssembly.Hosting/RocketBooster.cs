using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> ForAppDomain(AppDomain appDomain)
    {
        return _ => new ConventionContextBuilder(null).UseAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> ForAppDomain(
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
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> For(AppDomain appDomain)
    {
        return ForAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> For(
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAppDomain(appDomain, getConventions);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> For(
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAppDomain(AppDomain.CurrentDomain, getConventions);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> ForConventions(
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAppDomain(AppDomain.CurrentDomain, getConventions);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> ForAssemblies(IEnumerable<Assembly> assemblies)
    {
        return _ => new ConventionContextBuilder(null).UseAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> ForAssemblies(
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
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> For(IEnumerable<Assembly> assemblies)
    {
        return ForAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebAssemblyHostBuilder, ConventionContextBuilder> For(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAssemblies(assemblies, getConventions);
    }
}
