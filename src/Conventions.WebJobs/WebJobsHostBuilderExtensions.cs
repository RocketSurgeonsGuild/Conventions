using System;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions.WebJobs;

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
        [NotNull]
        public static IConventionHostBuilder ConfigureWebJobs(
            [NotNull] this IConventionHostBuilder container,
            WebJobsConventionDelegate @delegate
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}