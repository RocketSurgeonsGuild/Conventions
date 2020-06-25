using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.CommandLine;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class CommandLineHostBuilderExtensions
    {
        /// <summary>
        /// Configure the commandline delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IHostBuilder.</returns>
        public static IHostBuilder ConfigureCommandLine(
            [NotNull] this IHostBuilder container,
            CommandLineConventionDelegate @delegate
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.GetConventions().Scanner.AppendDelegate(@delegate);
            return container;
        }
        /// <summary>
        /// Configure the commandline delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureCommandLine(
            [NotNull] this IConventionHostBuilder container,
            CommandLineConventionDelegate @delegate
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