using System;
using JetBrains.Annotations;
using Rocket.Surgery.WebAssembly.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="ConventionContextBuilder" />
    /// </summary>
    public static class WebAssemblyHostingConventionExtensions
    {
        /// <summary>
        /// Configure the hosting delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static ConventionContextBuilder ConfigureHosting([NotNull] this ConventionContextBuilder container, WebAssemblyHostingConvention @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(@delegate);
            return container;
        }

        /// <summary>
        /// Configure the hosting delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static ConventionContextBuilder ConfigureHosting([NotNull] this ConventionContextBuilder container, Action<IWebAssemblyHostBuilder> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new WebAssemblyHostingConvention((context, builder) => @delegate(builder)));
            return container;
        }
    }
}