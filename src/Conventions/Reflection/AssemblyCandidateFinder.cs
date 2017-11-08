using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Class AssemblyCandidateFinder.
    /// </summary>
    /// TODO Edit XML Comment Template for AssemblyCandidateFinder
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
            _logger = logger;
        }

        public IEnumerable<Assembly> GetCandidateAssemblies(params string[] candidates)
        {
            return GetCandidateAssemblies(candidates.AsEnumerable());
        }

        /// <summary>
        /// Gets the candidate assemblies.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
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
            catch
            {
                _logger.LogWarning("Failed to load assembly {Assembly}", assemblyName.Name);
                return null;
            }
        }
    }
}
