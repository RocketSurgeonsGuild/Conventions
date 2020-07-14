using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> ForDependencyContext(
            DependencyContext dependencyContext,
            ILogger? diagnosticLogger = null,
            DiagnosticSource? diagnosticSource = null
        ) => builder =>
        {
            var b = RocketHostExtensions.GetOrCreateBuilder(builder);
            if (diagnosticSource != null)
            {
                b = b.With(diagnosticSource);
            }
            if (diagnosticLogger != null)
            {
                b = b.With(diagnosticLogger);
            }

            var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext, b.DiagnosticLogger);
            var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext, b.DiagnosticLogger);
            var scanner = RebuildScanner(b, assemblyCandidateFinder);

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
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> For(
            DependencyContext dependencyContext,
            ILogger? diagnosticLogger = null,
            DiagnosticSource? diagnosticSource = null
        ) => ForDependencyContext(dependencyContext, diagnosticLogger, diagnosticSource);

        /// <summary>
        /// Fors the application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> ForAppDomain(
            AppDomain appDomain,
            ILogger? diagnosticLogger = null,
            DiagnosticSource? diagnosticSource = null
        ) => builder =>
        {
            var b = RocketHostExtensions.GetOrCreateBuilder(builder);
            if (diagnosticSource != null)
            {
                b = b.With(diagnosticSource);
            }
            if (diagnosticLogger != null)
            {
                b = b.With(diagnosticLogger);
            }

            var assemblyCandidateFinder = new AppDomainAssemblyCandidateFinder(appDomain, b.DiagnosticLogger);
            var assemblyProvider = new AppDomainAssemblyProvider(appDomain, b.DiagnosticLogger);
            var scanner = RebuildScanner(b, assemblyCandidateFinder);
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
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> For(
            AppDomain appDomain,
            ILogger? diagnosticLogger = null,
            DiagnosticSource? diagnosticSource = null
        ) => ForAppDomain(appDomain, diagnosticLogger, diagnosticSource);

        /// <summary>
        /// Fors the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> ForAssemblies(
            IEnumerable<Assembly> assemblies,
            ILogger? diagnosticLogger = null,
            DiagnosticSource? diagnosticSource = null
        ) => builder =>
        {
            var b = RocketHostExtensions.GetOrCreateBuilder(builder);
            if (diagnosticSource != null)
            {
                b = b.With(diagnosticSource);
            }
            if (diagnosticLogger != null)
            {
                b = b.With(diagnosticLogger);
            }

            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            var assemblyCandidateFinder = new DefaultAssemblyCandidateFinder(enumerable, b.DiagnosticLogger);
            var assemblyProvider = new DefaultAssemblyProvider(enumerable, b.DiagnosticLogger);
            var scanner = RebuildScanner(b, assemblyCandidateFinder);
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
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IHostBuilder, IConventionHostBuilder> For(
            IEnumerable<Assembly> assemblies,
            ILogger? diagnosticLogger = null,
            DiagnosticSource? diagnosticSource = null
        ) => ForAssemblies(assemblies, diagnosticLogger, diagnosticSource);

        internal static IConventionScanner RebuildScanner(IConventionHostBuilder b, IAssemblyCandidateFinder assemblyCandidateFinder) => b.Scanner switch
        {
            SimpleConventionScanner simpleScanner => new SimpleConventionScanner(
                assemblyCandidateFinder,
                b.ServiceProperties,
                b.DiagnosticLogger,
                simpleScanner
            ) as IConventionScanner,
            BasicConventionScanner basicScanner => new BasicConventionScanner(basicScanner) as IConventionScanner,
            AggregateConventionScanner aggregateScanner => new AggregateConventionScanner(
                assemblyCandidateFinder,
                b.ServiceProperties,
                b.DiagnosticLogger,
                aggregateScanner
            ) as IConventionScanner,
            _ => new SimpleConventionScanner(assemblyCandidateFinder, b.ServiceProperties, b.DiagnosticLogger)
        };
    }
}