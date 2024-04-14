using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Logging;

/// <summary>
///     Register additional logging providers with the logging builder
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask LoggingAsyncConvention(
    IConventionContext context,
    IConfiguration configuration,
    ILoggingBuilder builder,
    CancellationToken cancellationToken
);