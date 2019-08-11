using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
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
            this IConventionHostBuilder container,
            RocketLoggingOptions options = null)
        {
            container.ServiceProperties[typeof(RocketLoggingOptions)] = options ?? new RocketLoggingOptions();
            container.Scanner.PrependConvention<LoggingServiceConvention>();
            return container;
        }
    }
}
