using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Clavus.DryIoc;

/// <summary>
///     Delegate ServicePartAction
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="container"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask<IContainer> DryIocAsyncConvention(
    IClavusContext context,
    IConfiguration configuration,
    IServiceCollection services,
    IContainer container,
    CancellationToken cancellationToken
);
