using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

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
        /// <param name="state">The state.</param>
        /// <returns>IConfigurationBuilder.</returns>
        [NotNull]
        public static IConfigurationBuilder AddApplicationState(
            this IConfigurationBuilder builder,
            [NotNull] IApplicationState state
        )
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            builder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.Debug)}"] =
                        state.Debug.ToString(CultureInfo.InvariantCulture),
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.Trace)}"] =
                        state.Trace.ToString(CultureInfo.InvariantCulture),
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.Verbose)}"] =
                        state.Verbose.ToString(CultureInfo.InvariantCulture),
                    [$"{nameof(ApplicationState)}:{nameof(ApplicationState.IsDefaultCommand)}"] =
                        state.IsDefaultCommand.ToString(CultureInfo.InvariantCulture)
                }
            );

            var logLevel = state.GetLogLevel();
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