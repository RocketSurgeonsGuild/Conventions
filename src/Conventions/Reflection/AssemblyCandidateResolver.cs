#if !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection
{
    class AssemblyCandidateResolver
    {
        private readonly IDictionary<string, Dependency> _dependencies;

        public AssemblyCandidateResolver(IReadOnlyList<Assembly> assemblies, ISet<string> referenceAssemblies)
        {
            var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
            foreach (var assembly in assemblies)
            {
                var key = assembly.GetName().Name;
                if (!dependenciesWithNoDuplicates.ContainsKey(key))
                {
                    dependenciesWithNoDuplicates.Add(key, CreateDependency(assembly.GetName(), referenceAssemblies));
                }
                foreach (var dependency in assembly.GetReferencedAssemblies())
                {
                    key = dependency.Name;
                    if (!dependenciesWithNoDuplicates.ContainsKey(key))
                    {
                        dependenciesWithNoDuplicates.Add(key, CreateDependency(dependency, referenceAssemblies));
                    }
                }
            }
            _dependencies = dependenciesWithNoDuplicates;
        }

        private Dependency CreateDependency(AssemblyName library, ISet<string> referenceAssemblies)
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
            // Prevents issues with looking at system assemblies
            if (dependency.StartsWith("System.") || dependency.StartsWith("mscorlib") || dependency.StartsWith("Microsoft."))
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

        public IEnumerable<Assembly> GetCandidates()
        {
            foreach (var dependency in _dependencies)
            {
                if (ComputeClassification(dependency.Key) == DependencyClassification.Candidate && dependency.Value.Assembly != null)
                {
                    yield return dependency.Value.Assembly;
                }
            }
        }

        private class Dependency
        {
            private readonly Lazy<Assembly> _assembly;
            private readonly AssemblyName _assemblyName;

            public Dependency(AssemblyName assemblyName, DependencyClassification classification)
            {
                _assemblyName = assemblyName;
                Classification = classification;
                _assembly = new Lazy<Assembly>(() =>
                {
                    try
                    {
                        return Assembly.Load(_assemblyName);
                    }
                    catch { }
                    return null;
                });
            }

            public Assembly Assembly => _assembly.Value;

            public DependencyClassification Classification { get; set; }

            public override string ToString()
            {
                return $"AssemblyName: {_assemblyName.Name}, Classification: {Classification}";
            }
        }

        private enum DependencyClassification
        {
            Unknown = 0,
            Candidate = 1,
            NotCandidate = 2,
            Reference = 3
        }
    }
}
#endif
