using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyModel;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// RuntimeLibraryCandidateResolver.
    /// </summary>
    class RuntimeLibraryCandidateResolver
    {
        private readonly IDictionary<string, Dependency> _dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeLibraryCandidateResolver" /> class.
        /// </summary>
        /// <param name="runtimeDependencies">The runtime dependencies.</param>
        /// <param name="referenceAssemblies">The reference assemblies.</param>
        public RuntimeLibraryCandidateResolver(IReadOnlyList<RuntimeLibrary> runtimeDependencies, ISet<string> referenceAssemblies)
        {
            var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
            foreach (var dependency in runtimeDependencies)
            {
                dependenciesWithNoDuplicates.Add(dependency.Name, CreateDependency(dependency, referenceAssemblies));
            }
            _dependencies = dependenciesWithNoDuplicates;
        }

        private Dependency CreateDependency(RuntimeLibrary library, ISet<string> referenceAssemblies)
        {
            var classification = DependencyClassification.Unknown;
            if (referenceAssemblies.Contains(library.Name))
            {
                classification = DependencyClassification.Reference;
            }

            return new Dependency(library, classification);
        }

        private DependencyClassification ComputeClassification(string dependency)
        {
            if (!_dependencies.TryGetValue(dependency, out var candidateEntry))
            {
                return DependencyClassification.Unknown;
            }

            if (candidateEntry.Classification != DependencyClassification.Unknown)
            {
                return candidateEntry.Classification;
            }
            else
            {
                var classification = DependencyClassification.NotCandidate;
                foreach (var candidateDependency in candidateEntry.Library.Dependencies)
                {
                    var dependencyClassification = ComputeClassification(candidateDependency.Name);
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
        }

        /// <summary>
        /// Gets the candidates.
        /// </summary>
        /// <returns>IEnumerable{RuntimeLibrary}</returns>
        public IEnumerable<RuntimeLibrary> GetCandidates()
        {
            foreach (var dependency in _dependencies)
            {
                if (ComputeClassification(dependency.Key) == DependencyClassification.Candidate)
                {
                    yield return dependency.Value.Library;
                }
            }
        }

        /// <summary>
        /// Dependency.
        /// </summary>
        private class Dependency
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Dependency" /> class.
            /// </summary>
            /// <param name="library">The library.</param>
            /// <param name="classification">The classification.</param>
            public Dependency(RuntimeLibrary library, DependencyClassification classification)
            {
                Library = library;
                Classification = classification;
            }

            /// <summary>
            /// Gets the library.
            /// </summary>
            /// <value>The library.</value>
            public RuntimeLibrary Library { get; }

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
                return $"Library: {Library.Name}, Classification: {Classification}";
            }
        }
    }

}
