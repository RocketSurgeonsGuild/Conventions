using Microsoft.Extensions.Hosting;
using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Extension methods for <see cref="IRocketEnvironment"/>.
    /// </summary>
    public static class RocketEnvironmentExtensions
    {
        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Development"/>.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment"/>.</param>
        /// <returns>True if the environment name is <see cref="EnvironmentName.Development"/>, otherwise false.</returns>
        public static bool IsDevelopment(this IRocketEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(RocketEnvironments.Development);
        }

        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Staging"/>.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment"/>.</param>
        /// <returns>True if the environment name is <see cref="EnvironmentName.Staging"/>, otherwise false.</returns>
        public static bool IsStaging(this IRocketEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(RocketEnvironments.Staging);
        }

        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Production"/>.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment"/>.</param>
        /// <returns>True if the environment name is <see cref="EnvironmentName.Production"/>, otherwise false.</returns>
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
        /// <param name="hostEnvironment">An instance of <see cref="IRocketEnvironment"/>.</param>
        /// <param name="environmentName">Environment name to validate against.</param>
        /// <returns>True if the specified name is the same as the current environment, otherwise false.</returns>
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
