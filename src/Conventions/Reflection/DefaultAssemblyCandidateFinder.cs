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
    /// Default assembly candidate finder that uses a list of assemblies
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.Reflection.IAssemblyCandidateFinder" />
    public class DefaultAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        /// <summary>
        /// The assemblies
        /// </summary>
        private readonly List<Assembly> _assemblies;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyCandidateFinder" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="logger">The logger.</param>
        public DefaultAssemblyCandidateFinder(IEnumerable<Assembly> assemblies, ILogger logger = null)
        {
            _assemblies = assemblies.ToList();
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
                .Where(x => x != null)
                .Reverse(),
                LogValue(value));
        }

        private Action<Assembly> LogValue(string[] candidates) =>
            value => _logger.LogDebug("[{AssemblyCandidateFinder}] Found candidate assembly {AssemblyName} for candidates {@Candidates}",
                nameof(DefaultAssemblyCandidateFinder),
                value.GetName().Name,
                candidates
            );
        private IEnumerable<Assembly> GetCandidateLibraries(string[] candidates)
        {
            if (candidates == null || !candidates.Any())
            {
                return Enumerable.Empty<Assembly>();
            }

            var candidatesResolver = new AssemblyCandidateResolver(_assemblies, new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase), _logger);
            return candidatesResolver.GetCandidates().Select(x => x.Assembly);
        }
    }
}
