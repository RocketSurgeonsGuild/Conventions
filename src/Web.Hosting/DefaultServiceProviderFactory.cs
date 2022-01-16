using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Web.Hosting;

internal class DefaultServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    private readonly IConventionServiceProviderFactory? _serviceProviderFactory;
    private readonly IConventionContext _conventionContext;

    public DefaultServiceProviderFactory(IConventionServiceProviderFactory? serviceProviderFactory, IConventionContext conventionContext)
    {
        _serviceProviderFactory = serviceProviderFactory;
        _conventionContext = conventionContext;
    }

    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return _serviceProviderFactory == null
            ? containerBuilder.BuildServiceProvider(_conventionContext.GetOrAdd(() => new ServiceProviderOptions()))
            : _serviceProviderFactory.CreateServiceProvider(containerBuilder, _conventionContext);
    }
}
