using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// IWebAssemblyHostingConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IWebAssemblyHostingConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        IWebAssemblyHostBuilder Builder { get; }
    }
}