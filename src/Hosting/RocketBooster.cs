using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketBooster.
    /// </summary>
    public static class RocketBooster
    {
        /// <summary>
        /// Fors the dependency context.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> ForDependencyContext(DependencyContext dependencyContext)
            => builder => RocketHostExtensions.SetupConventions(builder).UseDependencyContext(dependencyContext);

        /// <summary>
        /// Fors the specified dependency context.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> For(DependencyContext dependencyContext) => ForDependencyContext(dependencyContext);

        /// <summary>
        /// For the specified conventions
        /// </summary>
        /// <param name="conventionProvider">The conventions provider.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> ForConventions(Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider)
            => builder => RocketHostExtensions.SetupConventions(builder).WithConventionsFrom(conventionProvider);

        /// <summary>
        /// For the specified conventions
        /// </summary>
        /// <param name="conventionProvider">The conventions provider.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> For(Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider) => ForConventions(conventionProvider);

        /// <summary>
        /// Fors the application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> ForAppDomain(AppDomain appDomain)
            => builder => RocketHostExtensions.SetupConventions(builder).UseAppDomain(appDomain);

        /// <summary>
        /// Fors the specified application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> For(AppDomain appDomain) => ForAppDomain(appDomain);

        /// <summary>
        /// Fors the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> ForAssemblies(IEnumerable<Assembly> assemblies)
            => builder => RocketHostExtensions.SetupConventions(builder).UseAssemblies(assemblies);

        /// <summary>
        /// Fors the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<IHostBuilder, ConventionContextBuilder> For(IEnumerable<Assembly> assemblies) => ForAssemblies(assemblies);
    }
}