using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Aspire.Hosting.Testing;

/// <summary>
///     Class RocketBooster.
/// </summary>
[PublicAPI]
public static partial class RocketBooster
{
    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext)
    {
        return (_, _) => ValueTask.FromResult(new ConventionContextBuilder(new Dictionary<object, object>()).UseDependencyContext(dependencyContext));
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(IConventionFactory getConventions)
    {
        return (_, _) => ValueTask.FromResult(new ConventionContextBuilder(new Dictionary<object, object>()).UseConventionFactory(getConventions));
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain)
    {
        return (_, _) => ValueTask.FromResult(new ConventionContextBuilder(new Dictionary<object, object>()).UseAppDomain(appDomain));
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies)
    {
        return (_, _) => ValueTask.FromResult(new ConventionContextBuilder(new Dictionary<object, object>()).UseAssemblies(assemblies));
    }

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext)
    {
        return ForDependencyContext(dependencyContext);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain)
    {
        return ForAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies)
    {
        return ForAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the specified factory.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IConventionFactory factory)
    {
        return ForConventions(factory);
    }
}
