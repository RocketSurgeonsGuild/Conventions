using System;
using JetBrains.Annotations;
using Rocket.Surgery.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// LoggingExtensions.
    /// </summary>
    public static class LoggingHostBuilder2Extensions
    {
        /// <summary>
        /// Uses the logging.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="options">The options.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder UseLogging(
            [NotNull] this IConventionHostBuilder container,
            RocketLoggingOptions? options = null
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.ServiceProperties[typeof(RocketLoggingOptions)] = options ?? new RocketLoggingOptions();
            container.Scanner.PrependConvention<LoggingServiceConvention>();
            return container;
        }
    }
}