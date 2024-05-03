using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

/// <summary>
///     Delegate ServiceConventionAction
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="container"></param>
/// <param name="cancellationToken"></param>
[PublicAPI]
public delegate ValueTask<IContainer> DryIocAsyncConvention(
    IConventionContext context,
    IConfiguration configuration,
    IServiceCollection services,
    IContainer container,
    CancellationToken cancellationToken
);
