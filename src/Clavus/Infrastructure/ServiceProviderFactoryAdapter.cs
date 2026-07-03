using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Infrastructure;

internal delegate ValueTask<IServiceProviderFactory<object>> ServiceProviderFactoryAdapter(
    IClavusContext context,
    IServiceCollection services,
    CancellationToken cancellationToken
);
