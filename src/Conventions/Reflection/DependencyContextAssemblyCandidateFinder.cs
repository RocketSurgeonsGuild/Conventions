using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Assembly candidate finder that uses <see cref="DependencyContext" />
    /// Implements the <see cref="Rocket.Surgery.Conventions.Reflection.IAssemblyCandidateFinder" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.Reflection.IAssemblyCandidateFinder" />
    public class DependencyContextAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        private readonly DependencyContext _dependencyContext;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyCandidateFinder" /> class.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="logger">The logger.</param>
        public DependencyContextAssemblyCandidateFinder(DependencyContext dependencyContext, ILogger logger = null)
        {
            _dependencyContext = dependencyContext;
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Get the candidates for a given set
        /// </summary>
        /// <param name="candidates">The candidates as an enumerable</param>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        /// <inheritdoc />
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
        {
            var value = candidates as string[] ?? candidates?.ToArray() ?? Array.Empty<string>();
            return LoggingEnumerable.Create(GetCandidateLibraries(value)
                    .SelectMany(library =>
                        library.GetDefaultAssemblyNames(_dependencyContext))
                    .Select(TryLoad)
                    .Where(x => x != null)
                    .Reverse(),
                LogValue(value)
            );
        }

        private Action<Assembly> LogValue(string[] candidates) =>
            value => _logger.LogDebug("[{AssemblyCandidateFinder}] Found candidate assembly {AssemblyName} for candidates {@Candidates}",
                nameof(DependencyContextAssemblyCandidateFinder),
                value.GetName().Name,
                candidates
            );

        private IEnumerable<RuntimeLibrary> GetCandidateLibraries(string[] candidates)
        {
            if (candidates == null || !candidates.Any())
            {
                return Enumerable.Empty<RuntimeLibrary>();
            }

            var candidatesResolver = new RuntimeLibraryCandidateResolver(_dependencyContext.RuntimeLibraries, new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase));
            return candidatesResolver.GetCandidates();
        }

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
