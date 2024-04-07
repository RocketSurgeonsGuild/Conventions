using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.AspNetCore.Builder.WebApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ConventionsDelegate = System.Func<System.IServiceProvider, System.Collections.Generic.IEnumerable<Rocket.Surgery.Conventions.IConventionWithDependencies>>;

namespace Rocket.Surgery.Web.Hosting;

/// <summary>
///     Class RocketBooster.
/// </summary>
[PublicAPI]
public static class RocketBooster
{

    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext)
    {
        if (dependencyContext == null) throw new ArgumentNullException(nameof(dependencyContext));
        return (builder, cancellationToken) =>
               {
                   // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!).UseDependencyContext(dependencyContext);
                   return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
               };
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(ConventionsDelegate conventionProvider)
    {
        if (conventionProvider == null) throw new ArgumentNullException(nameof(conventionProvider));
        return (builder, cancellationToken) =>
               {
                   // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   #pragma warning disable RCS1249
                   var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!)
                                        #pragma warning restore RCS1249
                                        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                       .UseDependencyContext(DependencyContext.Default!)
                                       .WithConventionsFrom(conventionProvider);
                   return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
               };
    }


    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain)
    {
        if (appDomain == null) throw new ArgumentNullException(nameof(appDomain));
        return (builder, cancellationToken) =>
               {
                   // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   #pragma warning disable RCS1249
                   var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!).UseAppDomain(appDomain);
                   #pragma warning restore RCS1249
                   return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
               };
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        return (builder, cancellationToken) =>
               {
                   // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!).UseAssemblies(assemblies);
                   return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
               };
    }


    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext, ConventionsDelegate getConventions)
    {
        if (dependencyContext == null) throw new ArgumentNullException(nameof(dependencyContext));
        return ForDependencyContext(dependencyContext).WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext)
    {
        if (dependencyContext == null) throw new ArgumentNullException(nameof(dependencyContext));
        return ForDependencyContext(dependencyContext);
    }

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext, ConventionsDelegate getConventions)
    {
        if (dependencyContext == null) throw new ArgumentNullException(nameof(dependencyContext));
        return ForDependencyContext(dependencyContext, getConventions);
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(ConventionsDelegate conventionProvider)
    {
        if (conventionProvider == null) throw new ArgumentNullException(nameof(conventionProvider));
        return ForConventions(conventionProvider);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain, ConventionsDelegate getConventions)
    {
        if (appDomain == null) throw new ArgumentNullException(nameof(appDomain));
        return ForAppDomain(appDomain).WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain)
    {
        if (appDomain == null) throw new ArgumentNullException(nameof(appDomain));
        return ForAppDomain(appDomain);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain, ConventionsDelegate getConventions)
    {
        if (appDomain == null) throw new ArgumentNullException(nameof(appDomain));
        return ForAppDomain(appDomain, getConventions);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies, ConventionsDelegate getConventions)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        return ForAssemblies(assemblies).WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        return ForAssemblies(assemblies);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies, ConventionsDelegate getConventions)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        return ForAssemblies(assemblies, getConventions);
    }
}
