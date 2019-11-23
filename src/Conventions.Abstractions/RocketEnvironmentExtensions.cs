using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
#pragma warning disable RS0017

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
        public static bool IsDevelopment([NotNull] this IRocketEnvironment hostEnvironment)
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
        public static bool IsStaging([NotNull] this IRocketEnvironment hostEnvironment)
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
        public static bool IsProduction([NotNull] this IRocketEnvironment hostEnvironment)
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
            [NotNull] this IRocketEnvironment hostEnvironment,
            [NotNull] string environmentName
        )
        {
            // if (hostEnvironment == null)
            // {
            //     throw new ArgumentNullException(nameof(hostEnvironment));
            // }

            return string.Equals(
                hostEnvironment.EnvironmentName,
                environmentName,
                StringComparison.OrdinalIgnoreCase
            );
        }

        /// <summary>
        /// Converts the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>IRocketEnvironment.</returns>
#if NETCOREAPP3_0 || NETSTANDARD2_1
        [Obsolete("IHostingEnvironment is obsolete and will be removed in a future version. The recommended alternative is Microsoft.Extensions.Hosting.IHostEnvironment.", error: false)]
#endif
        [NotNull]
        public static IRocketEnvironment Convert([NotNull] this IHostingEnvironment environment)
        {
            return new RocketEnvironment(environment);
        }
#if NETCOREAPP3_0 || NETSTANDARD2_1
        /// <summary>
        /// Converts the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>IRocketEnvironment.</returns>
        [NotNull]
        public static IRocketEnvironment Convert([NotNull] this IHostEnvironment environment)
            => new RocketEnvironment(environment);
#endif
    }
}