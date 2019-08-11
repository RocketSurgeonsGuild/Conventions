using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// AssemblyCandidateResolver.
    /// </summary>
    class AssemblyCandidateResolver
    {
        private readonly ILogger _logger;
        private readonly IDictionary<string, Dependency> _dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCandidateResolver" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="referenceAssemblies">The reference assemblies.</param>
        /// <param name="logger">The logger.</param>
        public AssemblyCandidateResolver(IReadOnlyList<Assembly> assemblies, ISet<string> referenceAssemblies, ILogger logger)
        {
            _logger = logger;
            var processedAssemblies = new HashSet<Assembly>();
            var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
            foreach (var assembly in assemblies)
            {
                RecursiveAddDependencies(assembly, referenceAssemblies, dependenciesWithNoDuplicates, processedAssemblies);
            }
            _dependencies = dependenciesWithNoDuplicates;
        }

        private void RecursiveAddDependencies(Assembly assembly,
            ISet<string> referenceAssemblies,
            IDictionary<string, Dependency> dependenciesWithNoDuplicates,
            ISet<Assembly> processedAssemblies)
        {
            if (processedAssemblies.Contains(assembly)) return;
            processedAssemblies.Add(assembly);
            var key = assembly.GetName().Name;
            if (!dependenciesWithNoDuplicates.ContainsKey(key))
            {
                dependenciesWithNoDuplicates.Add(key, CreateDependency(assembly, referenceAssemblies));
            }

            foreach (var dependency in assembly.GetReferencedAssemblies())
            {
                if (dependency.Name.StartsWith("System.") || dependency.Name.StartsWith("Windows") || dependency.Name.StartsWith("mscorlib") || dependency.Name.StartsWith("Microsoft."))
                    continue;

                Assembly dependentAssembly;
                try
                {
                    dependentAssembly = Assembly.Load(dependency);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(0, e, "Unable to load assembly {Name}", dependency.Name);
                    continue;
                }
                RecursiveAddDependencies(dependentAssembly, referenceAssemblies, dependenciesWithNoDuplicates, processedAssemblies);
            }
        }

        private Dependency CreateDependency(Assembly library, ISet<string> referenceAssemblies)
        {
            var classification = DependencyClassification.Unknown;
            if (referenceAssemblies.Contains(library.GetName().Name))
            {
                classification = DependencyClassification.Reference;
            }

            return new Dependency(library, classification);
        }

        private DependencyClassification ComputeClassification(string dependency, ISet<string> processedAssemblies)
        {
            processedAssemblies.Add(dependency);
            // Prevents issues with looking at system assemblies
            if (dependency.StartsWith("System.") ||
                dependency.StartsWith("mscorlib") ||
                dependency.StartsWith("Microsoft.") ||
                dependency.StartsWith("DynamicProxyGenAssembly"))
            {
                return DependencyClassification.NotCandidate;
            }

            if (!_dependencies.TryGetValue(dependency, out var candidateEntry) || candidateEntry.Assembly == null)
            {
                return DependencyClassification.Unknown;
            }

            if (candidateEntry.Classification != DependencyClassification.Unknown)
            {
                return candidateEntry.Classification;
            }

            var classification = DependencyClassification.NotCandidate;

            foreach (var candidateDependency in candidateEntry.Assembly.GetReferencedAssemblies())
            {
                if (processedAssemblies.Contains(candidateDependency.Name))
                {
                    continue;
                }
                var dependencyClassification = ComputeClassification(candidateDependency.Name, processedAssemblies);
                if (dependencyClassification == DependencyClassification.Candidate ||
                    dependencyClassification == DependencyClassification.Reference)
                {
                    classification = DependencyClassification.Candidate;
                    break;
                }
            }

            candidateEntry.Classification = classification;

            return classification;
        }

        /// <summary>
        /// Gets the candidates.
        /// </summary>
        /// <returns>IEnumerable{Dependency}.</returns>
        public IEnumerable<Dependency> GetCandidates()
        {
            foreach (var dependency in _dependencies)
            {
                if (ComputeClassification(dependency.Key, new HashSet<string>()) == DependencyClassification.Candidate && dependency.Value.Assembly != null)
                {
                    yield return dependency.Value;
                }
            }
        }

        /// <summary>
        /// Dependency.
        /// </summary>
        internal class Dependency
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Dependency" /> class.
            /// </summary>
            /// <param name="assemblyName">Name of the assembly.</param>
            /// <param name="classification">The classification.</param>
            public Dependency(Assembly assemblyName, DependencyClassification classification)
            {
                Assembly = assemblyName;
                Classification = classification;
            }

            /// <summary>
            /// Gets the assembly.
            /// </summary>
            /// <value>The assembly.</value>
            public Assembly Assembly { get; }

            /// <summary>
            /// Gets or sets the classification.
            /// </summary>
            /// <value>The classification.</value>
            public DependencyClassification Classification { get; set; }

            /// <summary>
            /// Returns a <see cref="string" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="string" /> that represents this instance.</returns>
            public override string ToString()
            {
                return $"AssemblyName: {Assembly.GetName().Name}, Classification: {Classification}";
            }
        }
    }
}
