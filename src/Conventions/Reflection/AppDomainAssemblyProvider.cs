using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Assembly provider that uses <see cref="AppDomain" />
/// </summary>
/// <seealso cref="IAssemblyProvider" />
internal class AppDomainAssemblyProvider : IAssemblyProvider
{
    private readonly ILogger _logger;
    private readonly Lazy<IEnumerable<Assembly>> _assembles;

    private readonly Action<ILogger, string, string, Exception?> _logFoundAssembly = LoggerMessage.Define<string, string>(
        LogLevel.Debug, new EventId(1337), "[{AssemblyProvider}] Found assembly {AssemblyName}"
    );

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppDomainAssemblyProvider" /> class.
    /// </summary>
    /// <param name="appDomain">The application domain</param>
    /// <param name="logger">The logger to log information</param>
    public AppDomainAssemblyProvider(AppDomain? appDomain = null, ILogger? logger = null)
    {
        _assembles = new Lazy<IEnumerable<Assembly>>(
            () =>
                // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                ( appDomain ?? AppDomain.CurrentDomain ).GetAssemblies().Where(x => x != null!)
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
    ///     Gets the assemblies.
    /// </summary>
    /// <returns>IEnumerable{Assembly}.</returns>
    public IEnumerable<Assembly> GetAssemblies()
    {
        return LoggingEnumerable.Create(_assembles.Value, LogValue);
    }
}
