using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Assembly provider that uses <see cref="DependencyContext" />
///     Implements the <see cref="IAssemblyProvider" />
/// </summary>
/// <seealso cref="IAssemblyProvider" />
internal class DependencyContextAssemblyProvider : IAssemblyProvider
{
    private readonly ILogger _logger;
    private readonly Lazy<IEnumerable<Assembly>> _assembles;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DependencyContextAssemblyProvider" /> class.
    /// </summary>
    /// <param name="context">The dependency contenxt to list assemblies for.</param>
    /// <param name="logger">The logger to log out diagnostic information.</param>
    public DependencyContextAssemblyProvider(DependencyContext context, ILogger? logger = null)
    {
        _assembles = new Lazy<IEnumerable<Assembly>>(
            () =>
                context.GetDefaultAssemblyNames()
                       .Select(TryLoad)
                        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                       .Where(x => x != null!)
                       .ToArray()
        );
        _logger = logger ?? NullLogger.Instance;
    }

    private void LogValue(Assembly value)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _logger.FoundAssembly(nameof(DependencyContextAssemblyProvider), value.GetName().Name!);
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
            return default!;
        }
#pragma warning restore CA1031
    }

    /// <summary>
    ///     Gets the assemblies.
    /// </summary>
    /// <returns>IEnumerable{Assembly}.</returns>
    public IEnumerable<Assembly> GetAssemblies()
    {
        return LoggingEnumerable.Create(_assembles.Value, LogValue);
    }
}
