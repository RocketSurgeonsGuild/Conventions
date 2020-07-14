using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketHostBuilder.
    /// Implements the <see cref="ConventionHostBuilder{TSelf}" />
    /// </summary>
    /// <seealso cref="ConventionHostBuilder{IConventionHostBuilder}" />
    internal class RocketHostBuilder : ConventionHostBuilder<RocketHostBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketHostBuilder" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="serviceProperties">The service properties.</param>
        public RocketHostBuilder(
            IHostBuilder builder,
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            DiagnosticSource diagnosticSource,
            IServiceProviderDictionary serviceProperties
        ) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, serviceProperties) => ServiceProperties.Set(builder);

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketHostBuilder" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="serviceProperties">The service properties.</param>
        public RocketHostBuilder(
            IHostBuilder builder,
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            ILogger diagnosticLogger,
            IServiceProviderDictionary serviceProperties
        ) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticLogger, serviceProperties) => ServiceProperties.Set(builder);

        /// <summary>
        /// Withes the specified scanner.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal RocketHostBuilder With(IConventionScanner scanner)
        {
            ServiceProperties.Set(scanner);
            return this;
        }

        /// <summary>
        /// Withes the specified assembly candidate finder.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal RocketHostBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            ServiceProperties.Set(assemblyCandidateFinder);
            return this;
        }

        /// <summary>
        /// Withes the specified assembly provider.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal RocketHostBuilder With(IAssemblyProvider assemblyProvider)
        {
            ServiceProperties.Set(assemblyProvider);
            return this;
        }

        /// <summary>
        /// Withes the specified diagnostic source.
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal RocketHostBuilder With(DiagnosticSource diagnosticSource) => With(new DiagnosticLogger(diagnosticSource));

        /// <summary>
        /// Withes the specified diagnostic source.
        /// </summary>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>RocketHostBuilder.</returns>
        internal RocketHostBuilder With(ILogger diagnosticLogger)
        {
            ServiceProperties.Set(diagnosticLogger);
            return this;
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IHostBuilder Builder => ServiceProperties.Get<IHostBuilder>();
    }
}