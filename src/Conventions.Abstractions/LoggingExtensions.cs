using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions;

internal static class LoggingExtensions
{
    private static readonly Action<ILogger, string, string, string[], Exception?> _logFoundCandidateAssembly = LoggerMessage.Define<string, string, string[]>(
        LogLevel.Debug, new EventId(1337), "[{AssemblyCandidateFinder}] Found candidate assembly {AssemblyName} for candidates {@Candidates}"
    );

    public static void FoundCandidateAssembly(this ILogger logger, string provider, string assemblyName, string[] candidates)
    {
        _logFoundCandidateAssembly(logger, provider, assemblyName, candidates, null);
    }

    private static readonly Action<ILogger, string, string, Exception?> _logFoundAssembly = LoggerMessage.Define<string, string>(
        LogLevel.Debug, new EventId(1337), "[{AssemblyProvider}] Found assembly {AssemblyName}"
    );

    public static void FoundAssembly(this ILogger logger, string provider, string assemblyName)
    {
        _logFoundAssembly(logger, provider, assemblyName, null);
    }

    private static readonly Action<ILogger, string, Exception?> _logTryingToLoadAssembly = LoggerMessage.Define<string>(
        LogLevel.Debug, new EventId(1337), "Trying to load assembly {Assembly}"
    );

    public static void TryingToLoadAssembly(this ILogger logger, string assemblyName)
    {
        _logTryingToLoadAssembly(logger, assemblyName, null);
    }

    private static readonly Action<ILogger, string, Exception?> _logWarnFailedToLoadAssembly =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1337), "Failed to load assembly {Assembly}");

    public static void FailedToLoadAssembly(this ILogger logger, string assemblyName, Exception exception)
    {
        _logWarnFailedToLoadAssembly(logger, assemblyName, exception);
    }
}
