using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;
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
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForDependencyContext(DependencyContext dependencyContext, CancellationToken cancellationToken = default) =>
        builder =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!).UseDependencyContext(dependencyContext);
            return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
        };

    /// <summary>
    ///     Fors the dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForDependencyContext(
        DependencyContext dependencyContext,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions,
        CancellationToken cancellationToken = default
    ) =>
        async builder => (await ForDependencyContext(dependencyContext, cancellationToken)(builder)).WithConventionsFrom(getConventions);

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(DependencyContext dependencyContext, CancellationToken cancellationToken = default) => ForDependencyContext(dependencyContext, cancellationToken);

    /// <summary>
    ///     Fors the specified dependency context.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(
        DependencyContext dependencyContext,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions,
        CancellationToken cancellationToken = default
    ) =>
        ForDependencyContext(dependencyContext, getConventions, cancellationToken);

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForConventions(
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider,
        CancellationToken cancellationToken = default
    ) =>
        builder =>
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

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider) => ForConventions(conventionProvider);

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForAppDomain(AppDomain appDomain,
        CancellationToken cancellationToken = default) =>
        builder =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            #pragma warning disable RCS1249
            var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!).UseAppDomain(appDomain);
            #pragma warning restore RCS1249
            return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
        };

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForAppDomain(
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions,
        CancellationToken cancellationToken = default
    ) =>
        async builder => ( await ForAppDomain(appDomain)(builder)).WithConventionsFrom(getConventions);

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(AppDomain appDomain,
        CancellationToken cancellationToken = default) =>
        ForAppDomain(appDomain, cancellationToken);

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(
        AppDomain appDomain, Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions,
        CancellationToken cancellationToken = default
    ) =>
        ForAppDomain(appDomain, getConventions, cancellationToken);

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForAssemblies(IEnumerable<Assembly> assemblies,
        CancellationToken cancellationToken = default) =>
        builder =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            var contextBuilder = new ConventionContextBuilder(builder.Host.Properties!).UseAssemblies(assemblies);
            return RocketWebHostExtensions.Configure(builder, contextBuilder, cancellationToken);
        };

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> ForAssemblies(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions,
        CancellationToken cancellationToken = default
    ) =>
        async builder => (await ForAssemblies(assemblies, cancellationToken)(builder)).WithConventionsFrom(getConventions);

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(
        IEnumerable<Assembly> assemblies,
        CancellationToken cancellationToken = default) =>
        ForAssemblies(assemblies, cancellationToken);

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<WebApplicationBuilder, ValueTask<ConventionContextBuilder>> For(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions,
        CancellationToken cancellationToken = default
    ) =>
        ForAssemblies(assemblies, getConventions, cancellationToken);
}
