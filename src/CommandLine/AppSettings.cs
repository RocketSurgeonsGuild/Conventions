using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     Default class used for application settings
/// </summary>
public class AppSettings : CommandSettings
{
    /// <summary>
    ///     Gets the log.
    /// </summary>
    /// <value>The log.</value>
    [CommandOption("-l|--log|--loglevel")]
    [UsedImplicitly]
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is
    ///     verbose.
    /// </summary>
    /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
    [CommandOption("--verbose")]
    [Description("Verbose logging")]
    [UsedImplicitly]
    public bool Verbose
    {
        get => LogLevel == Microsoft.Extensions.Logging.LogLevel.Debug;
        set => LogLevel = value ? Microsoft.Extensions.Logging.LogLevel.Debug : LogLevel;
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is trace.
    /// </summary>
    /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
    [CommandOption("--trace")]
    [Description("Trace logging")]
    [UsedImplicitly]
    public bool Trace
    {
        get => LogLevel == Microsoft.Extensions.Logging.LogLevel.Trace;
        set => LogLevel = value ? Microsoft.Extensions.Logging.LogLevel.Trace : LogLevel;
    }
}
