using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Extension methods for <see cref="IConventionEnvironment"/>.
    /// </summary>
    public static class ConventionEnvironmentExtensions
    {
        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Development"/>.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IConventionEnvironment"/>.</param>
        /// <returns>True if the environment name is <see cref="EnvironmentName.Development"/>, otherwise false.</returns>
        public static bool IsDevelopment(this IConventionEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(ConventionEnvironments.Development);
        }

        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Staging"/>.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IConventionEnvironment"/>.</param>
        /// <returns>True if the environment name is <see cref="EnvironmentName.Staging"/>, otherwise false.</returns>
        public static bool IsStaging(this IConventionEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(ConventionEnvironments.Staging);
        }

        /// <summary>
        /// Checks if the current host environment name is <see cref="EnvironmentName.Production"/>.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IConventionEnvironment"/>.</param>
        /// <returns>True if the environment name is <see cref="EnvironmentName.Production"/>, otherwise false.</returns>
        public static bool IsProduction(this IConventionEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(ConventionEnvironments.Production);
        }

        /// <summary>
        /// Compares the current host environment name against the specified value.
        /// </summary>
        /// <param name="hostEnvironment">An instance of <see cref="IConventionEnvironment"/>.</param>
        /// <param name="environmentName">Environment name to validate against.</param>
        /// <returns>True if the specified name is the same as the current environment, otherwise false.</returns>
        public static bool IsEnvironment(
            this IConventionEnvironment hostEnvironment,
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
    }
}
