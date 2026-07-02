using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Clavus;

internal delegate ValueTask<IServiceProviderFactory<object>> ServiceProviderFactoryAdapter(
    IClavusContext context,
    IServiceCollection services,
    CancellationToken cancellationToken
);
