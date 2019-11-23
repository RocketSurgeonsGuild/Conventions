using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Assembly provider that uses <see cref="AppDomain" />
    /// </summary>
    /// <seealso cref="IAssemblyProvider" />
    public class AppDomainAssemblyProvider : IAssemblyProvider
    {
        private readonly ILogger _logger;
        private readonly Lazy<IEnumerable<Assembly>> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
        /// </summary>
        /// <param name="appDomain">The application domain</param>
        /// <param name="logger">The logger to log information</param>
        public AppDomainAssemblyProvider(AppDomain? appDomain = null, ILogger? logger = null)
        {
            _assembles = new Lazy<IEnumerable<Assembly>>(
                () =>
                    ( appDomain ?? AppDomain.CurrentDomain ).GetAssemblies().Where(x => x != null)
            );
            _logger = logger ?? NullLogger.Instance;
        }

        private void LogValue(Assembly value)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "[{AssemblyProvider}] Found assembly {AssemblyName}",
                    nameof(AppDomainAssemblyProvider),
                    value.GetName().Name
                );
            }
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable{Assembly}.</returns>
        public IEnumerable<Assembly> GetAssemblies() => LoggingEnumerable.Create(_assembles.Value, LogValue);
    }
}