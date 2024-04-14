using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Default assembly candidate finder that uses a list of assemblies
/// </summary>
/// <seealso cref="IAssemblyCandidateFinder" />
internal class DefaultAssemblyCandidateFinder : IAssemblyCandidateFinder
{
    /// <summary>
    ///     The assemblies
    /// </summary>
    private readonly List<Assembly> _assemblies;

    /// <summary>
    ///     The logger
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultAssemblyCandidateFinder" /> class.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="logger">The logger.</param>
    public DefaultAssemblyCandidateFinder(IEnumerable<Assembly> assemblies, ILogger? logger = null)
    {
        _assemblies = assemblies.ToList();
        _logger = logger ?? NullLogger.Instance;
    }

    private Action<Assembly> LogValue(string[] candidates)
    {
        return value =>
        {
            _logger.FoundCandidateAssembly(
                nameof(DefaultAssemblyCandidateFinder),
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                value.GetName().Name!,
                candidates
            );
        };
    }

    private IEnumerable<Assembly> GetCandidateLibraries(string[]? candidates)
    {
        if (candidates?.Any() != true)
        {
            return Enumerable.Empty<Assembly>();
        }

        var candidatesResolver = new AssemblyCandidateResolver(
            _assemblies,
            new HashSet<string?>(candidates, StringComparer.OrdinalIgnoreCase),
            _logger
        );
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return candidatesResolver.GetCandidates().Select(x => x.Assembly!);
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
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
               .Where(x => x != null!)
               .Reverse(),
            LogValue(value)
        );
    }
}
