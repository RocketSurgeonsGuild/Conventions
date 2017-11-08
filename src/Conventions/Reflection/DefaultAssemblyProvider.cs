using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Class DefaultAssemblyProvider.
    /// </summary>
    /// <seealso cref="IAssemblyProvider" />
    /// TODO Edit XML Comment Template for DefaultAssemblyProvider
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        private readonly ILogger _logger;
        private readonly Lazy<IEnumerable<Assembly>> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyProvider" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public DefaultAssemblyProvider(DependencyContext context, ILogger logger)
        {
            _logger = logger;
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
            catch
            {
                _logger.LogWarning("Failed to load assembly {Assembly}", assemblyName.Name);
                return null;
            }
        }
    }
}
