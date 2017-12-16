using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyModel;

namespace Rocket.Surgery.Conventions.Reflection
{
    class RuntimeLibraryCandidateResolver
    {
        private readonly IDictionary<string, Dependency> _dependencies;

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

        private class Dependency
        {
            public Dependency(RuntimeLibrary library, DependencyClassification classification)
            {
                Library = library;
                Classification = classification;
            }

            public RuntimeLibrary Library { get; }

            public DependencyClassification Classification { get; set; }

            public override string ToString()
            {
                return $"Library: {Library.Name}, Classification: {Classification}";
            }
        }
    }

}
