using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Infrastructure;

/// <summary>
///    A factory that provides a service provider factory from the convention context
/// </summary>
/// <param name="context">The convention context</param>
/// <param name="services">The service collection</param>
/// <param name="cancellationToken">The cancellation token</param>
public delegate ValueTask<IServiceProviderFactory<object>> ServiceProviderFactoryAdapter(
    IClavusContext context,
    IServiceCollection services,
    CancellationToken cancellationToken
);
