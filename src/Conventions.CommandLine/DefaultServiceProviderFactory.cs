using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class DefaultServiceProviderFactory : IConventionServiceProviderFactory
{
    private readonly IServiceProvider? _serviceProvider;

    public DefaultServiceProviderFactory()
    {
    }

    public DefaultServiceProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection services, IConventionContext conventionContext)
    {
        services.ApplyConventions(conventionContext);
        new LoggingBuilder(services).ApplyConventions(conventionContext);
        if (_serviceProvider is null)
            return services.BuildServiceProvider();
        return new FallbackServiceProvider(_serviceProvider, services.BuildServiceProvider());
    }
}
