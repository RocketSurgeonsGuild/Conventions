using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Clavus.Configuration;

/// <summary>
///     IConfigurationAsyncPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IConfigurationAsyncPart : IClavusPart
{
    /// <summary>
    ///     Register additional configuration providers with the configuration builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, IConfiguration configuration, IConfigurationBuilder builder, CancellationToken cancellationToken);
}
