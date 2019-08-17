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
    /// Assembly provider that uses <see cref="DependencyContext" />
    /// Implements the <see cref="IAssemblyProvider" />
    /// </summary>
    /// <seealso cref="IAssemblyProvider" />
    public class DependencyContextAssemblyProvider : IAssemblyProvider
    {
        private readonly ILogger _logger;
        private readonly Lazy<IEnumerable<Assembly>> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyProvider" /> class.
        /// </summary>
        /// <param name="context">The dependency contenxt to list assemblies for.</param>
        /// <param name="logger">The logger to log out diagnostic information.</param>
        public DependencyContextAssemblyProvider(DependencyContext context, ILogger? logger = null)
        {
            _assembles = new Lazy<IEnumerable<Assembly>>(() =>
                context.GetDefaultAssemblyNames()
                    .Select(TryLoad)
                    .Where(x => x != null)
                    .ToArray());
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable{Assembly}.</returns>
        public IEnumerable<Assembly> GetAssemblies() => LoggingEnumerable.Create(_assembles.Value, LogValue);

        private void LogValue(Assembly value)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("[{AssemblyProvider}] Found assembly {AssemblyName}",
                    nameof(DependencyContextAssemblyProvider),
                    value.GetName().Name
                );
            }
        }

        private Assembly TryLoad(AssemblyName assemblyName)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Trying to load assembly {Assembly}", assemblyName.Name);
            }
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning(0, e, "Failed to load assembly {Assembly}", assemblyName.Name);
                }
                return default!;
            }
        }
    }
}
