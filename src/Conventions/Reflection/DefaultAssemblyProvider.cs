using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Default assembly provider that uses a list of assemblies
///     Implements the <see cref="IAssemblyProvider" />
/// </summary>
/// <seealso cref="IAssemblyProvider" />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal partial class DefaultAssemblyProvider : IAssemblyProvider
{
    [LoggerMessage("[{AssemblyProvider}] Found assembly {AssemblyName}", EventId = 1337, Level = LogLevel.Debug)]
    private static partial void LogFoundAssembly(ILogger logger, string assemblyProvider, string? assemblyName);

    private readonly ILogger _logger;
    private readonly ImmutableArray<Assembly> _assembles;

    private readonly HashSet<string> _coreAssemblies =
    [
        "mscorlib",
        "netstandard",
        "System",
        "System.Core",
        "System.Runtime",
        "System.Private.CoreLib",
    ];

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
    /// </summary>
    /// <param name="assemblies">The assemblies</param>
    /// <param name="logger">The logger</param>
    public DefaultAssemblyProvider(IEnumerable<Assembly> assemblies, ILogger? logger = null)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _assembles = assemblies.Where(x => x != null!).ToImmutableArray();
        _logger = logger ?? NullLogger.Instance;
    }

    private void LogValue(Assembly value)
    {
        LogFoundAssembly(
            _logger,
            nameof(DefaultAssemblyProvider),
            value.GetName().Name
        );
    }

    private IEnumerable<Assembly> GetCandidateLibraries(HashSet<Assembly> candidates)
    {
        if (!candidates.Any())
        {
            return Enumerable.Empty<Assembly>();
        }

        // Sometimes all the assemblies are not loaded... so we kind of have to yolo it and try a few times until we get all of them
        var candidatesResolver = new AssemblyCandidateResolver(
            _assembles,
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

    /// <summary>
    ///     Gets the assemblies based on the given selector.
    /// </summary>
    /// <remarks>This method is normally used by the generated code however, for legacy support it is supported at runtime as well</remarks>
    /// <param name="action"></param>
    /// <param name="filePath"></param>
    /// <param name="memberName"></param>
    /// <param name="lineNumber"></param>
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
        var assemblies = selector.AllAssemblies
            ? _assembles
            : selector.AssemblyDependencies.Any()
                ? GetCandidateLibraries(selector.AssemblyDependencies)
                : selector.Assemblies;
        if (!selector.SystemAssemblies)
        {
            assemblies = assemblies.Where(z => !_coreAssemblies.Contains(z.GetName().Name ?? ""));
        }

        return assemblies;
    }

    /// <summary>
    ///     Get the full list of types using the given selector
    /// </summary>
    /// <param name="action"></param>
    /// <param name="filePath"></param>
    /// <param name="memberName"></param>
    /// <param name="lineNumber"></param>
    /// <returns></returns>
    public IEnumerable<Type> GetTypes(
        Func<ITypeProviderAssemblySelector, IEnumerable<Type>> action,
        [CallerFilePath]
        string filePath = "",
        [CallerMemberName]
        string memberName = "",
        [CallerLineNumber]
        int lineNumber = 0
    )
    {
        var assemblySelector = new AssemblyProviderAssemblySelector();
        action(assemblySelector);
        var assemblies = assemblySelector.AllAssemblies
            ? _assembles
            : assemblySelector.AssemblyDependencies.Any()
                ? GetCandidateLibraries(assemblySelector.AssemblyDependencies)
                : assemblySelector.Assemblies;
        return action(new TypeProviderAssemblySelector { Assemblies = assemblies, });
    }
}