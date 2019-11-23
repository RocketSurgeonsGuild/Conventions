using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// IHostingConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IHostingConventionContext : IConventionContext, IRocketHostBuilder { }
}