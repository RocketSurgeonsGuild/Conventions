using Microsoft.Extensions.Hosting;
using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Extension methods for <see cref="IRocketEnvironment" />.
    /// </summary>
    public static class RocketEnvironmentExtensions
    {
        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Development" />.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment" />.</param>
        /// <returns><c>true</c> if the specified host environment is development; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">hostEnvironment</exception>

        public static bool IsDevelopment(this IRocketEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(RocketEnvironments.Development);
        }

        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Staging" />.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment" />.</param>
        /// <returns><c>true</c> if the specified host environment is staging; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">hostEnvironment</exception>

        public static bool IsStaging(this IRocketEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(RocketEnvironments.Staging);
        }

        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Production" />.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment" />.</param>
        /// <returns><c>true</c> if the specified host environment is production; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">hostEnvironment</exception>

        public static bool IsProduction(this IRocketEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(RocketEnvironments.Production);
        }

        /// <summary>
        /// Compares the current host environment name against the specified value.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment" />.</param>
        /// <param name="environmentName">Environment name to validate against.</param>
        /// <returns><c>true</c> if the specified environment name is environment; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">hostEnvironment</exception>

        public static bool IsEnvironment(
            this IRocketEnvironment hostEnvironment,
            string environmentName)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return string.Equals(
                hostEnvironment.EnvironmentName,
                environmentName,
                StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Converts the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>IRocketEnvironment.</returns>
        public static IRocketEnvironment Convert(this IHostingEnvironment environment)
        {
            return new RocketEnvironment(environment);
        }
#if NETCOREAPP3_0
        public static IRocketEnvironment Convert(this IHostEnvironment environment)
        {
            return new RocketEnvironment(environment);            
        }
#endif
    }
}
