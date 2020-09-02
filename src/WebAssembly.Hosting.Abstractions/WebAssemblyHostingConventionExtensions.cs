using System;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class WebAssemblyHostingConventionExtensions
    {
        /// <summary>
        /// Configure the hosting delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureHosting(
            [NotNull] this IConventionHostBuilder container,
            WebAssemblyHostingConventionDelegate @delegate
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