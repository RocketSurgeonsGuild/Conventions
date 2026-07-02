using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Hosting;

/// <summary>
///     IHostApplicationAsyncPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IHostApplicationAsyncPart<in TBuilder> : IClavusPart
    where TBuilder : IHostApplicationBuilder
{
    /// <summary>
    ///     Register an event to happen when a host application is being configured
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, TBuilder builder, CancellationToken cancellationToken);
}
