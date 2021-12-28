#if NET6_0_OR_GREATER
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Web.Hosting;

/// <summary>
///     Class RocketBooster.
/// </summary>
public static class RocketBooster
{
    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForDependencyContext(DependencyContext dependencyContext)
    {
        return builder => RocketWebHostExtensions.SetupConventions(builder).UseDependencyContext(dependencyContext);
    }

    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForDependencyContext(
        DependencyContext dependencyContext,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return builder => ForDependencyContext(dependencyContext)(builder).WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(DependencyContext dependencyContext)
    {
        return ForDependencyContext(dependencyContext);
    }

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(
        DependencyContext dependencyContext,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForDependencyContext(dependencyContext, getConventions);
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForConventions(
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider
    )
    {
        return builder => RocketWebHostExtensions.SetupConventions(builder).WithConventionsFrom(conventionProvider);
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider)
    {
        return ForConventions(conventionProvider);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForAppDomain(AppDomain appDomain)
    {
        return builder => RocketWebHostExtensions.SetupConventions(builder).UseAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForAppDomain(
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
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(AppDomain appDomain)
    {
        return ForAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(
        AppDomain appDomain, Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAppDomain(appDomain, getConventions);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForAssemblies(IEnumerable<Assembly> assemblies)
    {
        return builder => RocketWebHostExtensions.SetupConventions(builder).UseAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> ForAssemblies(
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
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(IEnumerable<Assembly> assemblies)
    {
        return ForAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ConventionContextBuilder> For(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAssemblies(assemblies, getConventions);
    }
}
#endif
