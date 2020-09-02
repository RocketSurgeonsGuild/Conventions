using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.WebAssembly.Hosting
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
        /// <returns>Func&lt;IWebAssemblyHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IWebAssemblyHostBuilder, IConventionHostBuilder> ForDependencyContext(
            DependencyContext dependencyContext,
            ILogger? diagnosticLogger = null
        ) => builder =>
        {
            var b = RocketWebAssemblyExtensions.GetOrCreateBuilder(builder);
            if (diagnosticLogger != null)
            {
                b = b.With(diagnosticLogger);
            }

            var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext, b.DiagnosticLogger);
            var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext, b.DiagnosticLogger);
            var scanner = RebuildScanner(b, assemblyCandidateFinder);

            return RocketWebAssemblyExtensions.Swap(
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
        /// <returns>Func&lt;IWebAssemblyHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IWebAssemblyHostBuilder, IConventionHostBuilder> For(
            DependencyContext dependencyContext,
            ILogger? diagnosticLogger = null
        ) => ForDependencyContext(dependencyContext, diagnosticLogger);

        /// <summary>
        /// Fors the application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>Func&lt;IWebAssemblyHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IWebAssemblyHostBuilder, IConventionHostBuilder> ForAppDomain(
            AppDomain appDomain,
            ILogger? diagnosticLogger = null
        ) => builder =>
        {
            var b = RocketWebAssemblyExtensions.GetOrCreateBuilder(builder);
            if (diagnosticLogger != null)
            {
                b = b.With(diagnosticLogger);
            }

            var assemblyCandidateFinder = new AppDomainAssemblyCandidateFinder(appDomain, b.DiagnosticLogger);
            var assemblyProvider = new AppDomainAssemblyProvider(appDomain, b.DiagnosticLogger);
            var scanner = RebuildScanner(b, assemblyCandidateFinder);
            return RocketWebAssemblyExtensions.Swap(
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
        /// <returns>Func&lt;IWebAssemblyHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IWebAssemblyHostBuilder, IConventionHostBuilder> For(
            AppDomain appDomain,
            ILogger? diagnosticLogger = null
        ) => ForAppDomain(appDomain, diagnosticLogger);

        /// <summary>
        /// Fors the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>Func&lt;IWebAssemblyHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IWebAssemblyHostBuilder, IConventionHostBuilder> ForAssemblies(
            IEnumerable<Assembly> assemblies,
            ILogger? diagnosticLogger = null
        ) => builder =>
        {
            var b = RocketWebAssemblyExtensions.GetOrCreateBuilder(builder);
            if (diagnosticLogger != null)
            {
                b = b.With(diagnosticLogger);
            }

            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            var assemblyCandidateFinder = new DefaultAssemblyCandidateFinder(enumerable, b.DiagnosticLogger);
            var assemblyProvider = new DefaultAssemblyProvider(enumerable, b.DiagnosticLogger);
            var scanner = RebuildScanner(b, assemblyCandidateFinder);
            return RocketWebAssemblyExtensions.Swap(
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
        /// <returns>Func&lt;IWebAssemblyHostBuilder, IConventionHostBuilder&gt;.</returns>
        public static Func<IWebAssemblyHostBuilder, IConventionHostBuilder> For(
            IEnumerable<Assembly> assemblies,
            ILogger? diagnosticLogger = null
        ) => ForAssemblies(assemblies, diagnosticLogger);

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