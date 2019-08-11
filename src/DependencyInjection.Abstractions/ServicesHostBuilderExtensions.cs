using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class ServicesHostBuilderExtensions
    {
        /// <summary>
        /// Configure the services delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureServices(
            this IConventionHostBuilder container,
            ServiceConventionDelegate @delegate)
        {
            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}
