using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using System.Reflection;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// ApplicationState.
    /// </summary>
    public class ApplicationState
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IApplicationState" /> is verbose.
        /// </summary>
        /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
        [UsedImplicitly]
        public bool Verbose { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IApplicationState" /> is trace.
        /// </summary>
        /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
        [UsedImplicitly]
        public bool Trace { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IApplicationState" /> is debug.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        [UsedImplicitly]
        public bool Debug { get; }

        /// <summary>
        /// Gets the log level.
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
    };

}