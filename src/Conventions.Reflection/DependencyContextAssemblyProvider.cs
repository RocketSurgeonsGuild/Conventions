using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Type provider that uses <see cref="DependencyContext" />
///     Implements the <see cref="ICompiledTypeProvider" />
/// </summary>
/// <seealso cref="ICompiledTypeProvider" />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal class DependencyContextAssemblyProvider : ICompiledTypeProvider
{
    private readonly DependencyContext _dependencyContext;
    private readonly ILogger _logger;
    private readonly Lazy<ImmutableArray<Assembly>> _assembles;

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
    ///     Initializes a new instance of the <see cref="DependencyContextAssemblyProvider" /> class.
    /// </summary>
    /// <param name="context">The dependency contenxt to list assemblies for.</param>
    /// <param name="logger">The logger to log out diagnostic information.</param>
    public DependencyContextAssemblyProvider(DependencyContext context, ILogger? logger = null)
    {
        _dependencyContext = context;
        // ReSharper disable once NullableWarningSuppressionIsUsed
        _assembles = new(() => context.GetDefaultAssemblyNames().Select(TryLoad).Where(x => x != null).Select(z => z!).ToImmutableArray());
        _logger = logger ?? NullLogger.Instance;
    }

    private void LogValue(Assembly value)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _logger.FoundAssembly(nameof(DependencyContextAssemblyProvider), value.GetName().Name!);
    }

    private Assembly? TryLoad(AssemblyName assemblyName)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _logger.TryingToLoadAssembly(assemblyName.Name);

        try
        {
            return Assembly.Load(assemblyName);
        }
        #pragma warning disable CA1031
        catch (Exception e)
        {
            _logger.FailedToLoadAssembly(assemblyName.Name, e);
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            return default!;
        }
        #pragma warning restore CA1031
    }

    private IEnumerable<Assembly> GetCandidateLibraries(HashSet<Assembly> candidates)
    {
        if (candidates.Count == 0) return Enumerable.Empty<Assembly>();

        var candidatesResolver = new RuntimeLibraryCandidateResolver(
            _dependencyContext.RuntimeLibraries,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            new HashSet<string?>(candidates.Select(z => z.GetName().Name), StringComparer.OrdinalIgnoreCase)
        );
        return candidatesResolver
              .GetCandidates()
              .SelectMany(library => library.GetDefaultAssemblyNames(_dependencyContext))
              .Select(TryLoad)
              .Where(x => x != null)
               // ReSharper disable once NullableWarningSuppressionIsUsed
              .Select(z => z!)
              .Reverse();
    }

    /// <summary>
    ///     Gets the assemblies based on the given selector.
    /// </summary>
    /// <remarks>This method is normally used by the generated code however, for legacy support it is supported at runtime as well</remarks>
    /// <returns>IEnumerable{Assembly}.</returns>
    public IEnumerable<Assembly> GetAssemblies(
        Action<IReflectionAssemblySelector> action,
        [CallerLineNumber]
        int lineNumber = 0,
        [CallerFilePath]
        string filePath = "",
        [CallerArgumentExpression(nameof(action))]
        string argumentExpression = ""
    )
    {
        ArgumentNullException.ThrowIfNull(action);
        var selector = new AssemblyProviderAssemblySelector();
        action(selector);
        var assemblies = selector.AllAssemblies
            ? _assembles.Value
            : selector.AssemblyDependencies.Any()
                ? GetCandidateLibraries(selector.AssemblyDependencies)
                : selector.Assemblies;
        if (!selector.SystemAssemblies) assemblies = assemblies.Where(z => !_coreAssemblies.Contains(z.GetName().Name ?? ""));

        return assemblies;
    }

    public IEnumerable<Type> GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, int lineNumber = 0, string filePath = "", string argumentExpression = "") => throw new NotImplementedException();

    /// <summary>
    ///     Get the full list of types using the given selector
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Type> GetTypes(
        Func<IReflectionAssemblySelector, IEnumerable<Type>> selector,
        [CallerLineNumber]
        int lineNumber = 0,
        [CallerFilePath]
        string filePath = "",
        [CallerArgumentExpression(nameof(selector))]
        string argumentExpression = ""
    )
    {
        ArgumentNullException.ThrowIfNull(selector);
        var assemblySelector = new AssemblyProviderAssemblySelector();
        selector(assemblySelector);
        var assemblies = assemblySelector.AllAssemblies
            ? _assembles.Value
            : assemblySelector.AssemblyDependencies.Any()
                ? GetCandidateLibraries(assemblySelector.AssemblyDependencies)
                : assemblySelector.Assemblies;
        return selector(new TypeProviderAssemblySelector { Assemblies = assemblies, });
    }

    IServiceCollection ICompiledTypeProvider.Scan(
        IServiceCollection services,
        Action<IServiceDescriptorAssemblySelector> selector,
        int lineNumber,
        string filePath,
        string argumentExpression
    )
    {
        throw new NotImplementedException();
    }
}
