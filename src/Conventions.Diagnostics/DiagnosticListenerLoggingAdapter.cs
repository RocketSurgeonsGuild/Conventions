using System;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Diagnostics
{
    /// <summary>
    /// DiagnosticListenerLoggingAdapter.
    /// </summary>
    public class DiagnosticListenerLoggingAdapter
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticListenerLoggingAdapter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DiagnosticListenerLoggingAdapter(ILogger logger) => _logger = logger;

        /// <summary>
        /// Logs the other.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Other")]
        public void LogOther(LogLevel logLevel, EventId eventId, Exception exception, string message)
            => _logger.Log(logLevel, eventId, exception, message);

        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Trace")]
        public void LogTrace(EventId eventId, Exception exception, string message)
            => _logger.LogTrace(eventId, exception, message);

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Debug")]
        public void LogDebug(EventId eventId, Exception exception, string message)
            => _logger.LogDebug(eventId, exception, message);

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Information")]
        public void LogInformation(EventId eventId, Exception exception, string message)
            => _logger.LogInformation(eventId, exception, message);

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Warning")]
        public void LogWarning(EventId eventId, Exception exception, string message)
            => _logger.LogWarning(eventId, exception, message);

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Error")]
        public void LogError(EventId eventId, Exception exception, string message)
            => _logger.LogError(eventId, exception, message);

        /// <summary>
        /// Logs the critical.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        [DiagnosticName("Log.Critical")]
        public void LogCritical(EventId eventId, Exception exception, string message)
            => _logger.LogCritical(eventId, exception, message);
    }
}