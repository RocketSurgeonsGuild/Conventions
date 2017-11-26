using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Assembly provider that uses <see cref="DependencyContext"/>
    /// </summary>
    public class DependencyContextAssemblyProvider : IAssemblyProvider
    {
        private readonly ILogger _logger;
        private readonly Lazy<IEnumerable<Assembly>> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyProvider" /> class.
        /// </summary>
        /// <param name="context">The dependency contenxt to list assemblies for.</param>
        /// <param name="logger">The logger to log out diagnostic information.</param>
        public DependencyContextAssemblyProvider(DependencyContext context, ILogger logger = null)
        {
            _logger = logger ?? NullLogger.Instance;
            _assembles = new Lazy<IEnumerable<Assembly>>(() =>
                context.GetDefaultAssemblyNames()
                    .Select(TryLoad)
                    .Where(x => x != null));
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetAssemblies
        public IEnumerable<Assembly> GetAssemblies() => _assembles.Value;

        private Assembly TryLoad(AssemblyName assemblyName)
        {
            _logger.LogDebug("Trying to load assembly {Assembly}", assemblyName.Name);
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, "Failed to load assembly {Assembly}", assemblyName.Name);
                return default;
            }
        }
    }
}
