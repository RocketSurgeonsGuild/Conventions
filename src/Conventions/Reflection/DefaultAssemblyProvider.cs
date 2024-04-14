using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Default assembly provider that uses a list of assemblies
///     Implements the <see cref="IAssemblyProvider" />
/// </summary>
/// <seealso cref="IAssemblyProvider" />
internal class DefaultAssemblyProvider : IAssemblyProvider
{
    private readonly ILogger _logger;
    private readonly IEnumerable<Assembly> _assembles;

    private readonly Action<ILogger, string, string, Exception?> _logFoundAssembly = LoggerMessage.Define<string, string>(
        LogLevel.Debug, new EventId(1337), "[{AssemblyProvider}] Found assembly {AssemblyName}"
    );

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
    /// </summary>
    /// <param name="assemblies">The assemblies</param>
    /// <param name="logger">The logger</param>
    public DefaultAssemblyProvider(IEnumerable<Assembly> assemblies, ILogger? logger = null)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _assembles = assemblies.Where(x => x != null!).ToArray();
        _logger = logger ?? NullLogger.Instance;
    }

    private void LogValue(Assembly value)
    {
        _logFoundAssembly(
            _logger,
            nameof(DefaultAssemblyProvider),
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            value.GetName().Name!,
            null
        );
    }

    /// <summary>
    ///     Gets the assemblies.
    /// </summary>
    /// <returns>IEnumerable{Assembly}.</returns>
    public IEnumerable<Assembly> GetAssemblies()
    {
        return LoggingEnumerable.Create(_assembles, LogValue);
    }
}
