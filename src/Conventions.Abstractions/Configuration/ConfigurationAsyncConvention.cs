using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Register additional configuration providers with the configuration builder
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask ConfigurationAsyncConvention(IConventionContext context, IConfiguration configuration, IConfigurationBuilder builder, CancellationToken cancellationToken);
