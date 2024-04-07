﻿using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions;

internal static partial class LoggingExtensions
{
    [LoggerMessage(1337, LogLevel.Debug, "[{AssemblyCandidateFinder}] Found candidate assembly {AssemblyName} for candidates {@Candidates}")]
    public static partial void FoundCandidateAssembly(this ILogger logger, string provider, string assemblyName, string[] candidates);

    [LoggerMessage(1337, LogLevel.Debug, "[{AssemblyProvider}] Found assembly {AssemblyName}")]
    public static partial void FoundAssembly(this ILogger logger, string provider, string assemblyName);

    [LoggerMessage(1337, LogLevel.Debug, "Trying to load assembly {Assembly}")]
    public static partial void TryingToLoadAssembly(this ILogger logger, string assemblyName);

    [LoggerMessage(1337, LogLevel.Warning, "Failed to load assembly {Assembly}")]
    public static partial void FailedToLoadAssembly(this ILogger logger, string assemblyName, Exception exception);
}
