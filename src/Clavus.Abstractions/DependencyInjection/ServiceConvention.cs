using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Clavus.DependencyInjection;

/// <summary>
///     Register additional services with the service collection
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration"></param>
/// <param name="services"></param>
[PublicAPI]
public delegate void ServicePart(IClavusContext context, IConfiguration configuration, IServiceCollection services);
