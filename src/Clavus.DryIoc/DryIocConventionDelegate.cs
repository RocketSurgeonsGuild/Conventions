using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.DryIoc;

/// <summary>
///     Delegate ServicePartAction
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="container"></param>
[PublicAPI]
public delegate IContainer DryIocConvention(
    IClavusContext context,
    IConfiguration configuration,
    IServiceCollection services,
    IContainer container
);
