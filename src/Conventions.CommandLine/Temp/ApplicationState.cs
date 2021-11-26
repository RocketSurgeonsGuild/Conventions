using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     ApplicationState.
///     Implements the <see cref="IApplicationState" />
/// </summary>
/// <seealso cref="IApplicationState" />
[Command(UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
internal class ApplicationState : IApplicationState
{
    /// <summary>
    ///     Gets or sets the on parse delegates.
    /// </summary>
    /// <value>The on parse delegates.</value>
    public List<OnParseDelegate>? OnParseDelegates { get; internal set; } = new List<OnParseDelegate>();

    /// <summary>
    ///     Gets the log.
    /// </summary>
    /// <value>The log.</value>
    [Option(CommandOptionType.SingleValue, Description = "Log level", Inherited = true, ShowInHelpText = true)]
    [UsedImplicitly]
    public (bool HasValue, LogLevel Level) Log { get; }

    /// <summary>
    ///     Gets or sets the on run delegate.
    /// </summary>
    /// <value>The on run delegate.</value>
    internal OnRunDelegate? OnRunDelegate { get; set; }

    /// <summary>
    ///     Gets or sets the on run delegate.
    /// </summary>
    /// <value>The on run delegate.</value>
    internal OnRunAsyncDelegate? OnRunAsyncDelegate { get; set; }

    /// <summary>
    ///     Gets or sets the on run delegate.
    /// </summary>
    /// <value>The on run delegate.</value>
    internal OnRunAsyncCancellableDelegate? OnRunAsyncCancellableDelegate { get; set; }

    /// <summary>
    ///     Gets or sets the type of the on run.
    /// </summary>
    /// <value>The type of the on run.</value>
    internal Type? OnRunType { get; set; }

    /// <summary>
    ///     Gets or sets the type of the on run.
    /// </summary>
    /// <value>The type of the on run.</value>
    internal Type? OnRunAsyncType { get; set; }

    /// <summary>
    ///     Gets the remaining arguments.
    /// </summary>
    /// <value>The remaining arguments.</value>
    public string[] RemainingArguments { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     Gets a value indicating whether this <see cref="IApplicationState" /> is
    ///     verbose.
    /// </summary>
    /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
    [Option(CommandOptionType.NoValue, Description = "Verbose logging", Inherited = true, ShowInHelpText = true)]
    [UsedImplicitly]
    public bool Verbose { get; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="IApplicationState" /> is trace.
    /// </summary>
    /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
    [Option(CommandOptionType.NoValue, Description = "Trace logging", Inherited = true, ShowInHelpText = true)]
    [UsedImplicitly]
    public bool Trace { get; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="IApplicationState" /> is debug.
    /// </summary>
    /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
    [Option(CommandOptionType.NoValue, Description = "Debug logging", Inherited = true, ShowInHelpText = true)]
    [UsedImplicitly]
    public bool Debug { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is default command.
    /// </summary>
    /// <value><c>true</c> if this instance is default command; otherwise, <c>false</c>.</value>
    public bool IsDefaultCommand { get; internal set; }

    /// <summary>
    ///     Gets the log level.
    /// </summary>
    /// <returns>LogLevel.</returns>
    public LogLevel? GetLogLevel()
    {
        if (Log.HasValue)
        {
            return Log.Level;
        }

        if (Verbose || Trace)
        {
            return LogLevel.Trace;
        }

        if (Debug)
        {
            return LogLevel.Debug;
        }

        return null;
    }
}
