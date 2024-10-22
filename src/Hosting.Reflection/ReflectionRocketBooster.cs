using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using AppDelegate =
    System.Func<Microsoft.Extensions.Hosting.IHostApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Hosting.Reflection;

/// <summary>
///     Class RocketBooster.
/// </summary>
[PublicAPI]
public static partial class ReflectionRocketBooster
{
    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext, params ConventionCategory[] categories)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties, categories).UseDependencyContext(dependencyContext));
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain, params ConventionCategory[] categories)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties, categories).UseAppDomain(appDomain));
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies, params ConventionCategory[] categories)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties, categories).UseAssemblies(assemblies));
    }

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext, params ConventionCategory[] categories)
    {
        return ForDependencyContext(dependencyContext);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain, params ConventionCategory[] categories)
    {
        return ForAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies, params ConventionCategory[] categories)
    {
        return ForAssemblies(assemblies);
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(IConventionFactory getConventions, params ConventionCategory[] categories)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties, categories).UseConventionFactory(getConventions));
    }

    /// <summary>
    ///     Fors the specified factory.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IConventionFactory factory, params ConventionCategory[] categories)
    {
        return ForConventions(factory);
    }
}
