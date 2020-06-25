using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

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
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> ForDependencyContext(
            DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        ) => builder =>
        {
            var b = RocketHostExtensions.GetOrCreateBuilder(builder);
            if (diagnosticSource != null)
            {
                b = b.With(diagnosticSource);
            }

            var logger = new DiagnosticLogger(b.DiagnosticSource);
            var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext, logger);
            var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext, logger);
            var scanner = new SimpleConventionScanner(assemblyCandidateFinder, b.ServiceProperties, logger);
            return RocketHostExtensions.Swap(
                b,
                b
                   .With(assemblyCandidateFinder)
                   .With(assemblyProvider)
                   .With(scanner)
            );
        };

        /// <summary>
        /// Fors the specified dependency context.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> For(
            DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        ) => ForDependencyContext(dependencyContext, diagnosticSource);

        /// <summary>
        /// Fors the application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> ForAppDomain(
            AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        ) => builder =>
        {
            var b = RocketHostExtensions.GetOrCreateBuilder(builder);
            if (diagnosticSource != null)
            {
                b = b.With(diagnosticSource);
            }

            var logger = new DiagnosticLogger(b.DiagnosticSource);
            var assemblyCandidateFinder = new AppDomainAssemblyCandidateFinder(appDomain, logger);
            var assemblyProvider = new AppDomainAssemblyProvider(appDomain, logger);
            var scanner = new SimpleConventionScanner(assemblyCandidateFinder, b.ServiceProperties, logger);
            return RocketHostExtensions.Swap(
                b,
                b
                   .With(assemblyCandidateFinder)
                   .With(assemblyProvider)
                   .With(scanner)
            );
        };

        /// <summary>
        /// Fors the specified application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> For(
            AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        ) => ForAppDomain(appDomain, diagnosticSource);

        /// <summary>
        /// Fors the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> ForAssemblies(
            IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        ) => builder =>
        {
            var b = RocketHostExtensions.GetOrCreateBuilder(builder);
            if (diagnosticSource != null)
            {
                b = b.With(diagnosticSource);
            }

            var logger = new DiagnosticLogger(b.DiagnosticSource);
            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            var assemblyCandidateFinder = new DefaultAssemblyCandidateFinder(enumerable, logger);
            var assemblyProvider = new DefaultAssemblyProvider(enumerable, logger);
            var scanner = new SimpleConventionScanner(assemblyCandidateFinder, b.ServiceProperties, logger);
            return RocketHostExtensions.Swap(
                b,
                b
                   .With(assemblyCandidateFinder)
                   .With(assemblyProvider)
                   .With(scanner)
            );
        };

        /// <summary>
        /// Fors the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> For(
            IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        ) => ForAssemblies(assemblies, diagnosticSource);
    }
}