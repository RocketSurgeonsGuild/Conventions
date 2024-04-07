using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Adapters;

internal interface IServiceFactoryAdapter
{
    ValueTask<object> CreateBuilder(IServiceCollection services);
    ValueTask<IServiceProvider> CreateServiceProvider(object containerBuilder);
}

internal class ServiceFactoryAdapter<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory) : IServiceFactoryAdapter
    where TContainerBuilder : notnull
{
    private readonly IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory =
        serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));

    #if NET6_0_OR_GREATER
    public ValueTask<object> CreateBuilder(IServiceCollection services)
    {
        return ValueTask.FromResult<object>(_serviceProviderFactory.CreateBuilder(services));
    }

    public ValueTask<IServiceProvider> CreateServiceProvider(object containerBuilder)
    {
        return ValueTask.FromResult<IServiceProvider>(_serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder));
    }
    #else
    public async ValueTask<object> CreateBuilder(IServiceCollection services)
    {
        return _serviceProviderFactory.CreateBuilder(services);
    }

    public async ValueTask<IServiceProvider> CreateServiceProvider(object containerBuilder)
    {
        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
    #endif
}
