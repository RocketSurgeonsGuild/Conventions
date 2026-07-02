using Aspire.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire;

/// <summary>
///     ILoggingPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IDistributedApplicationAsyncConvention : IClavusPart
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, IDistributedApplicationBuilder builder, CancellationToken cancellationToken);
}
