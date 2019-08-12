using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Logging
{
    /// <summary>
    /// RocketLoggingOptions.
    /// </summary>
    public class RocketLoggingOptions
    {
        /// <summary>
        /// Determines how the loglevel is captured, defautlts to the value that can be set into the configuraiton
        /// IApplicationState:LogLevel
        /// </summary>
        /// <value>The get log level.</value>
        public Func<ILoggingConventionContext, LogLevel> GetLogLevel { get; set; } = context => context.Configuration.GetValue<LogLevel?>("ApplicationState:LogLevel", LogLevel.Information) ?? LogLevel.Information;
    }
}
