using Aspire.Hosting.Testing;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire.Testing;

/// <summary>
///     ILoggingPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IDistributedApplicationTestingAsyncConvention : IClavusPart
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, IDistributedApplicationTestingBuilder builder, CancellationToken cancellationToken);
}
