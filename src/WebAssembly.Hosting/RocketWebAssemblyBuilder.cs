using System.Diagnostics;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// Class RocketWebAssemblyBuilder.
    /// Implements the <see cref="ConventionHostBuilder{TSelf}" />
    /// </summary>
    /// <seealso cref="ConventionHostBuilder{IConventionHostBuilder}" />
    internal class RocketWebAssemblyBuilder : ConventionHostBuilder<RocketWebAssemblyBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketWebAssemblyBuilder" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <param name="serviceProperties">The service properties.</param>
        public RocketWebAssemblyBuilder(
            IWebAssemblyHostBuilder builder,
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
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal RocketWebAssemblyBuilder With(IConventionScanner scanner)
        {
            ServiceProperties.Set(scanner);
            return this;
        }

        /// <summary>
        /// Withes the specified assembly candidate finder.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal RocketWebAssemblyBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            ServiceProperties.Set(assemblyCandidateFinder);
            return this;
        }

        /// <summary>
        /// Withes the specified assembly provider.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal RocketWebAssemblyBuilder With(IAssemblyProvider assemblyProvider)
        {
            ServiceProperties.Set(assemblyProvider);
            return this;
        }

        /// <summary>
        /// Withes the specified diagnostic source.
        /// </summary>
        /// <param name="diagnosticLogger">The diagnostic logger.</param>
        /// <returns>RocketWebAssemblyBuilder.</returns>
        internal RocketWebAssemblyBuilder With(ILogger diagnosticLogger)
        {
            ServiceProperties.Set(diagnosticLogger);
            return this;
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IWebAssemblyHostBuilder Builder => ServiceProperties.Get<IWebAssemblyHostBuilder>()!;
    }
}