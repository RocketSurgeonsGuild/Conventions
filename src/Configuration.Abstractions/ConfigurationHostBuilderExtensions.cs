using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class ConfigurationHostBuilderExtensions
    {
        /// <summary>
        /// Configure the configuration delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureConfiguration(
            this IConventionHostBuilder container,
            ConfigurationConventionDelegate @delegate)
        {
            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}
