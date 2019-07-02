using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Default assembly provider that uses a list of assemblies
    /// Implements the <see cref="IAssemblyProvider" />
    /// </summary>
    /// <seealso cref="IAssemblyProvider" />
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<Assembly> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies</param>
        /// <param name="logger">The logger</param>
        public DefaultAssemblyProvider(IEnumerable<Assembly> assemblies, ILogger logger = null)
        {
            _assembles = assemblies.Where(x => x != null).ToArray();
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable{Assembly}.</returns>
        public IEnumerable<Assembly> GetAssemblies() => LoggingEnumerable.Create(_assembles, LogValue);

        private void LogValue(Assembly value) =>
            _logger.LogDebug("[{AssemblyProvider}] Found assembly {AssemblyName}",
                nameof(DefaultAssemblyProvider),
                value.GetName().Name
            );
    }
}
