using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Diagnostics;

/// <summary>
///     DiagnosticLogger.
///     Implements the <see cref="ILogger" />
/// </summary>
/// <seealso cref="ILogger" />
public class DiagnosticLogger : ILogger
{
    /// <summary>
    ///     The names
    /// </summary>
    internal static readonly IReadOnlyDictionary<LogLevel, string> Names =
        Enum.GetValues(typeof(LogLevel))
            .Cast<LogLevel>()
            .ToDictionary(x => x, x => $"Log.{x}");

    private static string GetName(LogLevel logLevel)
    {
        return Names.TryGetValue(logLevel, out var value) ? value : "Log.Other";
    }

    /// <summary>
    ///     The underlying diagnostic source
    /// </summary>
    public DiagnosticSource DiagnosticSource { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiagnosticLogger" /> class.
    /// </summary>
    /// <param name="diagnosticSource">The diagnostic source.</param>
    public DiagnosticLogger(DiagnosticSource diagnosticSource)
    {
        DiagnosticSource = diagnosticSource;
    }

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
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        DiagnosticSource.Write(
            GetName(logLevel),
            new
            {
                logLevel,
                eventId,
                state = (object?)state,
                exception,
                message = formatter(state, exception)
            }
        );
    }

    /// <summary>
    ///     Checks if the given <paramref name="logLevel" /> is enabled.
    /// </summary>
    /// <param name="logLevel">level to be checked.</param>
    /// <returns><c>true</c> if enabled.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return DiagnosticSource.IsEnabled(GetName(logLevel));
    }

    /// <summary>
    ///     Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the t state.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state)
    {
#pragma warning disable CA2000
        var activity = DiagnosticSource.StartActivity(new Activity("Scope"), state);
#pragma warning restore CA2000
        return new Disposable(DiagnosticSource, activity, state!);
    }

    /// <summary>
    ///     Disposable.
    ///     Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    private class Disposable : IDisposable
    {
        private readonly DiagnosticSource _diagnosticSource;
        private readonly Activity _activity;
        private readonly object _state;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Disposable" /> class.
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="activity">The activity.</param>
        /// <param name="state">The state.</param>
        public Disposable(DiagnosticSource diagnosticSource, Activity activity, object state)
        {
            _diagnosticSource = diagnosticSource;
            _activity = activity;
            _state = state;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _diagnosticSource.StopActivity(_activity, _state);
        }
    }
}
