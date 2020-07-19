using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// ApplicationStateExtensions.
    /// </summary>
    public static class ApplicationStateExtensions
    {
        /// <summary>
        /// Adds the state of the application.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="parseResult">The parse result.</param>
        /// <returns>IConfigurationBuilder.</returns>
        [NotNull]
        public static IConfigurationBuilder AddApplicationState(
            this IConfigurationBuilder builder,
            [NotNull] ParseResult parseResult
        )
        {
            if (parseResult == null)
            {
                throw new ArgumentNullException(nameof(parseResult));
            }

            var debug = parseResult.ValueForOption<bool>("--debug");
            var trace = parseResult.ValueForOption<bool>("--trace");
            var verbose = parseResult.ValueForOption<bool>("--verbose");
            var logLevel = parseResult.ValueForOption<LogLevel?>("--log-level");
            builder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.Debug)}"] =
                        debug.ToString(CultureInfo.InvariantCulture),
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.Trace)}"] =
                        trace.ToString(CultureInfo.InvariantCulture),
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.Verbose)}"] =
                        verbose.ToString(CultureInfo.InvariantCulture)
                }
            );

            if (logLevel.HasValue)
            {
                builder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        [$"{nameof(ApplicationState)}:LogLevel"] = logLevel.ToString()
                    }
                );
            }

            return builder;
        }
    }
}