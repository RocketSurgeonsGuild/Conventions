using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions;

internal delegate ValueTask<IServiceProviderFactory<object>> ServiceProviderFactoryAdapter(
    IConventionContext context,
    IServiceCollection services,
    CancellationToken cancellationToken
);
