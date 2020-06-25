using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Logging;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// LoggingExtensions.
    /// </summary>
    public static class UseLoggingHostBuilderExtensions
    {
        /// <summary>
        /// Uses the logging.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="options">The options.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder UseLogging(
            [NotNull] this IHostBuilder container,
            RocketLoggingOptions? options = null
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.GetConventions().UseLogging(options);
            return container;
        }

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