using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.DependencyInjection
{
    /// <summary>
    /// IServiceConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IServiceConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull] IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull] IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        [NotNull] IServiceCollection Services { get; }

        /// <summary>
        /// Gets the on build.
        /// </summary>
        /// <value>The on build.</value>
        [NotNull] IObservable<IServiceProvider> OnBuild { get; }
    }
}