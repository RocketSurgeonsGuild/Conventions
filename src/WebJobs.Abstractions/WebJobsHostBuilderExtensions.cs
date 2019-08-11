using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.WebJobs;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class WebJobsHostBuilderExtensions
    {
        /// <summary>
        /// Configure the web jobs delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureWebJobs(
            this IConventionHostBuilder container,
            WebJobsConventionDelegate @delegate)
        {
            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}
