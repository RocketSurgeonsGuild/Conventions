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
    /// Default assembly candidate finder that uses <see cref="DependencyContext"/>
    /// </summary>
    public class AssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        private readonly DependencyContext _dependencyContext;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCandidateFinder" /> class.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="logger">The logger.</param>
        public AssemblyCandidateFinder(DependencyContext dependencyContext, ILogger logger)
        {
            _dependencyContext = dependencyContext;
            _logger = logger ?? NullLogger.Instance;
        }

        /// <inheritdoc />
        public IEnumerable<Assembly> GetCandidateAssemblies(string candidate, params string[] candidates)
        {
            return GetCandidateAssemblies(new [] { candidate }.Concat(candidates));
        }

        /// <inheritdoc />
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
        {
            return GetCandidateLibraries(candidates.ToArray())
                .SelectMany(library =>
                    library.GetDefaultAssemblyNames(_dependencyContext))
                .Select(TryLoad)
                .Where(x => x != null);
        }

        internal IEnumerable<RuntimeLibrary> GetCandidateLibraries(string[] candidates)
        {
            if (!candidates.Any())
            {
                return Enumerable.Empty<RuntimeLibrary>();
            }

            var candidatesResolver = new CandidateResolver(_dependencyContext.RuntimeLibraries, new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase));
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
