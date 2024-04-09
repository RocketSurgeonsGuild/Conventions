using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Assembly provider that uses <see cref="AppDomain" />
/// </summary>
/// <seealso cref="IAssemblyProvider" />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal class AppDomainAssemblyProvider : IAssemblyProvider
{
    private readonly ILogger _logger;
    private readonly Lazy<ImmutableArray<Assembly>> _assembles;

    private readonly Action<ILogger, string, string, Exception?> _logFoundAssembly = LoggerMessage.Define<string, string>(
        LogLevel.Debug,
        new EventId(1337),
        "[{AssemblyProvider}] Found assembly {AssemblyName}"
    );

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
    /// </summary>
    /// <param name="appDomain">The application domain</param>
    /// <param name="logger">The logger to log information</param>
    public AppDomainAssemblyProvider(AppDomain? appDomain = null, ILogger? logger = null)
    {
        _assembles = new(
            () =>
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                ( appDomain ?? AppDomain.CurrentDomain ).GetAssemblies().Where(x => x != null!).ToImmutableArray()
        );
        _logger = logger ?? NullLogger.Instance;
    }

    private void LogValue(Assembly value)
    {
        _logFoundAssembly(
            _logger,
            nameof(AppDomainAssemblyProvider),
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            value.GetName().Name!,
            null
        );
    }

    /// <summary>
    ///     Gets the assemblies based on the given selector.
    /// </summary>
    /// <remarks>This method is normally used by the generated code however, for legacy support it is supported at runtime as well</remarks>
    /// <returns>IEnumerable{Assembly}.</returns>
    public IEnumerable<Assembly> GetAssemblies(
        Action<IAssemblyProviderAssemblySelector> action,
        [CallerFilePath]
        string filePath = "",
        [CallerMemberName]
        string memberName = "",
        [CallerLineNumber]
        int lineNumber = 0
    )
    {
        var selector = new AssemblyProviderAssemblySelector();
        action(selector);
        return LoggingEnumerable.Create(
            selector.AllAssemblies
                ? _assembles.Value
                : selector.AssemblyDependencies.Any()
                    ? GetCandidateLibraries(selector.AssemblyDependencies)
                    : selector.Assemblies,
            LogValue
        );
    }

    /// <summary>
    ///   Get the full list of types using the given selector
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="filePath"></param>
    /// <param name="memberName"></param>
    /// <param name="lineNumber"></param>
    /// <returns></returns>
    public IEnumerable<Type> GetTypes(
        Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector,
        [CallerFilePath]
        string filePath = "",
        [CallerMemberName]
        string memberName = "",
        [CallerLineNumber]
        int lineNumber = 0
    ) => selector(new TypeProviderAssemblySelector());

    private IEnumerable<Assembly> GetCandidateLibraries(HashSet<Assembly> candidates)
    {
        if (!candidates.Any())
        {
            return Enumerable.Empty<Assembly>();
        }

        // Sometimes all the assemblies are not loaded... so we kind of have to yolo it and try a few times until we get all of them
        var candidatesResolver = new AssemblyCandidateResolver(
            _assembles.Value,
            new HashSet<string?>(candidates.Select(z => z.GetName().Name), StringComparer.OrdinalIgnoreCase),
            _logger
        );
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return candidatesResolver
              .GetCandidates()
              .Where(x => x.Assembly is { })
              .Select(x => x.Assembly!)
              .Reverse();
    }
}
