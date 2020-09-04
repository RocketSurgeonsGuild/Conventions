using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Delegate HostingConventionAction
    /// </summary>
    /// <param name="context">The context.</param>
    public delegate void HostingConvention([NotNull] IConventionContext context, [NotNull] IHostBuilder builder);
}