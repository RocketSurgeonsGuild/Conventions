using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Diagnostics;

/// <summary>
///     DiagnosticLogger.
///     Implements the <see cref="ILogger" />
/// </summary>
/// <seealso cref="ILogger" />
/// <remarks>
///     Initializes a new instance of the <see cref="DiagnosticLogger" /> class.
/// </remarks>
/// <param name="diagnosticSource">The diagnostic source.</param>
[RequiresUnreferencedCode("DiagnosticLogger is used for diagnostic logging and may not work in all environments")]
public class DiagnosticLogger(DiagnosticSource diagnosticSource) : ILogger
{
    /// <summary>
    ///     The names
    /// </summary>
    internal static readonly IReadOnlyDictionary<LogLevel, string> Names =
        Enum
           .GetValues<LogLevel>()
           .Cast<LogLevel>()
           .ToDictionary(x => x, x => $"LogLevel.{x}");

    private static string GetName(LogLevel logLevel) => Names.TryGetValue(logLevel, out var value) ? value : "LogLevel.Other";

    /// <summary>
    ///     The underlying diagnostic source
    /// </summary>
    public DiagnosticSource DiagnosticSource { get; } = diagnosticSource;

    /// <summary>
    ///     Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the t state.</typeparam>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">
    ///     Function to create a <c>string</c> message of the <paramref name="state" /> and
    ///     <paramref name="exception" />.
    /// </param>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        ArgumentNullException.ThrowIfNull(formatter);

        DiagnosticSource.Write(
            GetName(logLevel),
            new
            {
                logLevel,
                eventId,
                state = (object?)state,
                exception,
                message = formatter(state, exception),
            }
        );
    }

    /// <summary>
    ///     Checks if the given <paramref name="logLevel" /> is enabled.
    /// </summary>
    /// <param name="logLevel">level to be checked.</param>
    /// <returns><c>true</c> if enabled.</returns>
    public bool IsEnabled(LogLevel logLevel) => DiagnosticSource.IsEnabled(GetName(logLevel));

    /// <summary>
    ///     Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the t state.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
#pragma warning disable CA2000
        var activity = DiagnosticSource.StartActivity(new("Scope"), state);
#pragma warning restore CA2000
        return new Disposable(DiagnosticSource, activity, state);
    }

    /// <summary>
    ///     Disposable.
    ///     Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <remarks>
    ///     Initializes a new instance of the <see cref="Disposable" /> class.
    /// </remarks>
    /// <param name="diagnosticSource">The diagnostic source.</param>
    /// <param name="activity">The activity.</param>
    /// <param name="state">The state.</param>
    [RequiresUnreferencedCode("DiagnosticLogger is used for diagnostic logging and may not work in all environments")]
    private class Disposable(DiagnosticSource diagnosticSource, Activity activity, object state) : IDisposable
    {
        private readonly DiagnosticSource _diagnosticSource = diagnosticSource;
        private readonly Activity _activity = activity;
        private readonly object _state = state;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _diagnosticSource.StopActivity(_activity, _state);
    }
}
