using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Logging
{
    /// <summary>
    /// RocketLoggingOptions.
    /// </summary>
    public class RocketLoggingOptions
    {
        /// <summary>
        /// Determines how the loglevel is captured, defaults to the value that can be set into the configuration
        /// IApplicationState:LogLevel
        /// </summary>
        /// <value>The get log level.</value>
        public Func<ILoggingConventionContext, LogLevel?> GetLogLevel { get; set; } = context
            => context.Configuration.GetValue<LogLevel?>("ApplicationState:LogLevel");
    }
}