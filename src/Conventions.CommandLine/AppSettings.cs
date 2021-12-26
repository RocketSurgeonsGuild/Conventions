using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

public class AppSettings : CommandSettings
{
    /// <summary>
    ///     Gets the log.
    /// </summary>
    /// <value>The log.</value>
    [CommandOption("-l|--log")]
    [UsedImplicitly]
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is
    ///     verbose.
    /// </summary>
    /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
    [CommandOption("-v|--verbose")]
    [Description("Verbose logging")]
    [UsedImplicitly]
    public bool Verbose
    {
        get => LogLevel == global::Microsoft.Extensions.Logging.LogLevel.Debug;
        set => LogLevel = value ? global::Microsoft.Extensions.Logging.LogLevel.Debug : LogLevel;
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is trace.
    /// </summary>
    /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
    [CommandOption("-t|--trace")]
    [Description("Trace logging")]
    [UsedImplicitly]
    public bool Trace
    {
        get => LogLevel == global::Microsoft.Extensions.Logging.LogLevel.Trace;
        set => LogLevel = value ? global::Microsoft.Extensions.Logging.LogLevel.Trace : LogLevel;
    }
}