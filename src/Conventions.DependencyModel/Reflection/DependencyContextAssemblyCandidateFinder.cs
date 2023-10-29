using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Assembly candidate finder that uses <see cref="DependencyContext" />
///     Implements the <see cref="IAssemblyCandidateFinder" />
/// </summary>
/// <seealso cref="IAssemblyCandidateFinder" />
internal class DependencyContextAssemblyCandidateFinder : IAssemblyCandidateFinder
{
    private readonly DependencyContext _dependencyContext;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DependencyContextAssemblyCandidateFinder" /> class.
    /// </summary>
    /// <param name="dependencyContext">The dependency context.</param>
    /// <param name="logger">The logger.</param>
    public DependencyContextAssemblyCandidateFinder(DependencyContext dependencyContext, ILogger? logger = null)
    {
        _dependencyContext = dependencyContext;
        _logger = logger ?? NullLogger.Instance;
    }

    private Action<Assembly> LogValue(string[] candidates)
    {
        return value =>
        {
            _logger.FoundCandidateAssembly(
                nameof(DependencyContextAssemblyCandidateFinder),
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                value.GetName().Name!,
                candidates
            );
        };
    }

    private IEnumerable<RuntimeLibrary> GetCandidateLibraries(string[]? candidates)
    {
        if (candidates == null || candidates.Length == 0)
        {
            return Enumerable.Empty<RuntimeLibrary>();
        }

        var candidatesResolver = new RuntimeLibraryCandidateResolver(
            _dependencyContext.RuntimeLibraries,
            new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase)
        );
        return candidatesResolver.GetCandidates();
    }

    private Assembly TryLoad(AssemblyName assemblyName)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _logger.TryingToLoadAssembly(assemblyName.Name!);

        try
        {
            return Assembly.Load(assemblyName);
        }
#pragma warning disable CA1031
        catch (Exception e)
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            _logger.FailedToLoadAssembly(assemblyName.Name!, e);
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            return default!;
        }
#pragma warning restore CA1031
    }

    /// <summary>
    ///     Get the candidates for a given set
    /// </summary>
    /// <param name="candidates">The candidates as an enumerable</param>
    /// <returns>IEnumerable{Assembly}.</returns>
    /// <inheritdoc />
    public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
    {
        var value = candidates as string[] ?? candidates.ToArray();
        return LoggingEnumerable.Create(
            GetCandidateLibraries(value)
               .SelectMany(
                    library =>
                        library.GetDefaultAssemblyNames(_dependencyContext)
                )
               .Select(TryLoad)
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
               .Where(x => x != null!)
               .Reverse(),
            LogValue(value)
        );
    }
}
