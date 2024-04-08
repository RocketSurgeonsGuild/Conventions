using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.Extensions.Hosting.IHostApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Hosting;

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
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext, ConventionProviderFactory getConventions)
    {
        return (builder, _) => ValueTask.FromResult(
                   new ConventionContextBuilder(builder.Properties)
                      .UseDependencyContext(dependencyContext)
                      .WithConventionsFrom(getConventions)
               );
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(ConventionProviderFactory getConventions)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties).WithConventionsFrom(getConventions));
    }


    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain, ConventionProviderFactory getConventions)
    {
        return (builder, _) => ValueTask.FromResult(
                   new ConventionContextBuilder(builder.Properties)
                      .UseAppDomain(appDomain)
                      .WithConventionsFrom(getConventions)
               );
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies, ConventionProviderFactory getConventions)
    {
        return (builder, _) => ValueTask.FromResult(
                   new ConventionContextBuilder(builder.Properties)
                      .UseAssemblies(assemblies)
                      .WithConventionsFrom(getConventions)
               );
    }

    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext)
    {
        return ForDependencyContext(dependencyContext, empty);
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
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext, ConventionProviderFactory getConventions)
    {
        return ForDependencyContext(dependencyContext, getConventions);
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(ConventionProviderFactory conventionProvider)
    {
        return ForConventions(conventionProvider);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain)
    {
        return ForAppDomain(appDomain, empty);
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
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain, ConventionProviderFactory getConventions)
    {
        return ForAppDomain(appDomain, getConventions);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies)
    {
        return ForAssemblies(assemblies, empty);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies, ConventionProviderFactory getConventions)
    {
        return ForAssemblies(assemblies, getConventions);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies)
    {
        return ForAssemblies(assemblies, empty);
    }

    private static ConventionProviderFactory empty = _ => Enumerable.Empty<IConventionWithDependencies>();
}
