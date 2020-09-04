using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="IConvention" />
    public interface IWebAssemblyHostingConvention : IConvention
    {
        /// <summary>
        /// Register additional logging providers with the logging builder
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="builder"></param>
        void Register([NotNull] IConventionContext context, [NotNull] IWebAssemblyHostBuilder builder);
    }
}