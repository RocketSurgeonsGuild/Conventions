using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Assembly provider that uses <see cref="AppDomain"/>
    /// </summary>
    public class AppDomainAssemblyProvider : IAssemblyProvider
    {
        private readonly ILogger _logger;
        private readonly Lazy<IEnumerable<Assembly>> _assembles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
        /// </summary>
        /// <param name="appDomain">The application domain</param>
        /// <param name="logger">The logger to log information</param>
        public AppDomainAssemblyProvider(AppDomain appDomain = null, ILogger logger = null)
        {
            _assembles = new Lazy<IEnumerable<Assembly>>(() =>
                (appDomain ?? AppDomain.CurrentDomain).GetAssemblies().Where(x => x != null));
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// TODO Edit XML Comment Template for GetAssemblies
        public IEnumerable<Assembly> GetAssemblies() => LoggingEnumerable.Create(_assembles.Value, LogValue);

        private void LogValue(Assembly value) =>
            _logger.LogDebug(0, "[{AssemblyProvider}] Found assembly {AssemblyName}",
                nameof(AppDomainAssemblyProvider),
                value.GetName().Name
            );
    }
}
