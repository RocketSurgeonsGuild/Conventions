using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// IHostingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IHostingConventionContext}" />
    public interface IHostingConvention : IConvention<IHostingConventionContext> { }
}