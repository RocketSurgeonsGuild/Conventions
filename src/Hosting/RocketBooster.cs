#if NET6_0_OR_GREATER
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.Extensions.Hosting.IHostApplicationBuilder, System.Threading.CancellationToken, System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;
using ConventionsDelegate =
    System.Func<System.IServiceProvider, System.Collections.Generic.IEnumerable<Rocket.Surgery.Conventions.IConventionWithDependencies>>;

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
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext)
        => (builder, cancellationToken) =>
           {
               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
               var contextBuilder = new ConventionContextBuilder(builder.Properties!).UseDependencyContext(dependencyContext);
               return RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
           };

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(ConventionsDelegate conventionProvider) =>
        (builder, cancellationToken) =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            #pragma warning disable RCS1249
            var contextBuilder = new ConventionContextBuilder(builder.Properties!)
                                 #pragma warning restore RCS1249
                                 // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                .UseDependencyContext(DependencyContext.Default!)
                                .WithConventionsFrom(conventionProvider);
            return RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
        };


    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain) =>
        (builder, cancellationToken) =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            #pragma warning disable RCS1249
            var contextBuilder = new ConventionContextBuilder(builder.Properties!).UseAppDomain(appDomain);
            #pragma warning restore RCS1249
            return RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
        };

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies) =>
        (builder, cancellationToken) =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            var contextBuilder = new ConventionContextBuilder(builder.Properties!).UseAssemblies(assemblies);
            return RocketHostApplicationExtensions.Configure(builder, contextBuilder, cancellationToken);
        };

    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForDependencyContext(DependencyContext dependencyContext, ConventionsDelegate getConventions) =>
        ForDependencyContext(dependencyContext).WithConventionsFrom(getConventions);

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext) => ForDependencyContext(dependencyContext);

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(DependencyContext dependencyContext, ConventionsDelegate getConventions) =>
        ForDependencyContext(dependencyContext, getConventions);

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(ConventionsDelegate conventionProvider) => ForConventions(conventionProvider);

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAppDomain(AppDomain appDomain, ConventionsDelegate getConventions) =>
        ForAppDomain(appDomain).WithConventionsFrom(getConventions);

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain) => ForAppDomain(appDomain);

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(AppDomain appDomain, ConventionsDelegate getConventions) => ForAppDomain(appDomain, getConventions);

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForAssemblies(IEnumerable<Assembly> assemblies, ConventionsDelegate getConventions) =>
        ForAssemblies(assemblies).WithConventionsFrom(getConventions);

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies) => ForAssemblies(assemblies);

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IEnumerable<Assembly> assemblies, ConventionsDelegate getConventions) => ForAssemblies(assemblies, getConventions);
}
#endif
