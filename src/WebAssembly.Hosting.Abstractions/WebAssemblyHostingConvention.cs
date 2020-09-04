using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// Delegate HostingConventionAction
    /// </summary>
    /// <param name="context">The context.</param>
    public delegate void WebAssemblyHostingConvention([NotNull] IConventionContext context, [NotNull] IWebAssemblyHostBuilder builder);
}