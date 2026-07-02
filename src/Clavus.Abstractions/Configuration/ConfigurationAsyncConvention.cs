using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Clavus.Configuration;

/// <summary>
///     Register additional configuration providers with the configuration builder
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask ConfigurationAsyncPart(
    IClavusContext context,
    IConfiguration configuration,
    IConfigurationBuilder builder,
    CancellationToken cancellationToken
);
