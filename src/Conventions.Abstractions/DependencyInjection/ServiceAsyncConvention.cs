using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DependencyInjection;

/// <summary>
///     Register additional services with the service collection
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask ServiceAsyncConvention(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken);
