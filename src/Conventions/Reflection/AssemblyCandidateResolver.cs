using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     AssemblyCandidateResolver.
/// </summary>
internal class AssemblyCandidateResolver
{
    private static Dependency CreateDependency(Assembly library, ISet<string?> referenceAssemblies)
    {
        var classification = DependencyClassification.Unknown;
        if (referenceAssemblies.Contains(library.GetName().Name))
        {
            classification = DependencyClassification.Reference;
        }

        return new Dependency(library, classification);
    }

    private readonly ILogger _logger;
    private readonly IDictionary<string, Dependency> _dependencies;

    public static IEnumerable<Assembly> GetReferencedAssemblies(Assembly assembly)
    {
        var resolver = new AssemblyCandidateResolver(new[] { assembly }, new HashSet<string?>(StringComparer.OrdinalIgnoreCase), NullLogger.Instance);
        return resolver.GetAssemblies();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AssemblyCandidateResolver" /> class.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="referenceAssemblies">The reference assemblies.</param>
    /// <param name="logger">The logger.</param>
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public AssemblyCandidateResolver(
        IReadOnlyList<Assembly> assemblies,
        ISet<string?> referenceAssemblies,
        ILogger logger
    )
    {
        _logger = logger;
        var processedAssemblies = new HashSet<Assembly>();
        var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
        foreach (var assembly in assemblies)
        {
            RecursiveAddDependencies(
                assembly,
                referenceAssemblies,
                dependenciesWithNoDuplicates,
                processedAssemblies
            );
        }

        _dependencies = dependenciesWithNoDuplicates;
    }

    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetReferencedAssemblies()")]
    private void RecursiveAddDependencies(
        Assembly assembly,
        ISet<string?> referenceAssemblies,
        IDictionary<string, Dependency> dependenciesWithNoDuplicates,
        ISet<Assembly> processedAssemblies
    )
    {
        if (!processedAssemblies.Add(assembly))
        {
            return;
        }

        var key = assembly.GetName().Name;
        if (!string.IsNullOrWhiteSpace(key) && !dependenciesWithNoDuplicates.ContainsKey(key))
        {
            dependenciesWithNoDuplicates.Add(key, CreateDependency(assembly, referenceAssemblies));
        }

        foreach (var dependency in assembly.GetReferencedAssemblies())
        {
            if (dependency.Name?.StartsWith("System.", StringComparison.OrdinalIgnoreCase) == true ||
                dependency.Name?.StartsWith("Windows", StringComparison.OrdinalIgnoreCase) == true ||
                dependency.Name?.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) == true ||
                dependency.Name?.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) == true)
            {
                continue;
            }

            Assembly dependentAssembly;
            try
            {
                dependentAssembly = Assembly.Load(dependency);
            }
#pragma warning disable CA1031
            catch (Exception e)
            {
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                _logger.FailedToLoadAssembly(dependency.Name!, e);

                continue;
            }
#pragma warning restore CA1031

            RecursiveAddDependencies(
                dependentAssembly,
                referenceAssemblies,
                dependenciesWithNoDuplicates,
                processedAssemblies
            );
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    private DependencyClassification ComputeClassification(string dependency, ISet<string?>? processedAssemblies = null)
    {
        processedAssemblies ??= new HashSet<string?>();
        processedAssemblies.Add(dependency);
        // Prevents issues with looking at system assemblies
        if (dependency.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
            dependency.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) ||
            dependency.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) ||
            dependency.StartsWith("Windows", StringComparison.OrdinalIgnoreCase) ||
            dependency.StartsWith("DynamicProxyGenAssembly", StringComparison.OrdinalIgnoreCase))
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
            if (string.IsNullOrWhiteSpace(candidateDependency.Name) ||
                processedAssemblies.Contains(candidateDependency.Name))
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
    /// Gets all the related assemblies
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Assembly> GetAssemblies()
    {
        foreach (var dependency in _dependencies)
        {
            if (ComputeClassification(dependency.Key) is DependencyClassification.Candidate or DependencyClassification.NotCandidate && dependency.Value.Assembly is { })
            {
                yield return dependency.Value.Assembly;
            }
        }
    }

    /// <summary>
    ///     Gets the candidates.
    /// </summary>
    /// <returns>IEnumerable{Dependency}.</returns>
    public IEnumerable<Dependency> GetCandidates()
    {
        foreach (var dependency in _dependencies)
        {
            if (ComputeClassification(dependency.Key) is DependencyClassification.Candidate && dependency.Value.Assembly is { })
            {
                yield return dependency.Value;
            }
        }
    }

    /// <summary>
    ///     Dependency.
    /// </summary>
    internal class Dependency
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Dependency" /> class.
        /// </summary>
        /// <param name="assembly">Name of the assembly.</param>
        /// <param name="classification">The classification.</param>
        public Dependency(Assembly assembly, DependencyClassification classification)
        {
            Assembly = assembly;
            Classification = classification;
        }

        /// <summary>
        ///     Gets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        public Assembly? Assembly { get; }

        /// <summary>
        ///     Gets or sets the classification.
        /// </summary>
        /// <value>The classification.</value>
        public DependencyClassification Classification { get; set; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"AssemblyName: {Assembly?.GetName().Name}, Classification: {Classification}";
        }
    }
}
