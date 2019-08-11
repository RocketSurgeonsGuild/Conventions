using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class LoggingHostBuilderExtensions
    {
        /// <summary>
        /// Configure the logging delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureLogging(
            this IConventionHostBuilder container,
            LoggingConventionDelegate @delegate)
        {
            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}
