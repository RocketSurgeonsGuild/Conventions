using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Logging
{
    /// <summary>
    /// ILoggingConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="Microsoft.Extensions.Logging.ILoggingBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggingBuilder" />
    public interface ILoggingConventionContext : IConventionContext, Microsoft.Extensions.Logging.ILoggingBuilder
    {
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
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }
    }
}