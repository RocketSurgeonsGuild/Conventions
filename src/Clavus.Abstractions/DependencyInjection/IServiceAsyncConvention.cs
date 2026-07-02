using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Clavus.DependencyInjection;

/// <summary>
///     IServiceAsyncPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IServiceAsyncPart : IClavusPart
{
    /// <summary>
    ///     Register additional services with the service collection
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken);
}
