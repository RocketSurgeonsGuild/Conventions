using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Adapters;

internal interface IServiceFactoryAdapter
{
    object CreateBuilder(IServiceCollection services);

    IServiceProvider CreateServiceProvider(object containerBuilder);
}

internal class ServiceFactoryAdapter<TContainerBuilder> : IServiceFactoryAdapter
    where TContainerBuilder : notnull
{
    private readonly IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory;

    public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
    {
        _serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
    }

    public object CreateBuilder(IServiceCollection services)
    {
        return _serviceProviderFactory.CreateBuilder(services);
    }

    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
