using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Clavus.Logging;

/// <summary>
///     Register additional logging providers with the logging builder
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask LoggingAsyncPart(
    IClavusContext context,
    IConfiguration configuration,
    ILoggingBuilder builder,
    CancellationToken cancellationToken
);
