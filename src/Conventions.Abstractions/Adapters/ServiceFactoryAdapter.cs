using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Adapters;

internal interface IServiceFactoryAdapter
{
    ValueTask<object> CreateBuilder(IServiceCollection services, CancellationToken cancellationToken);
    ValueTask<IServiceProvider> CreateServiceProvider(object containerBuilder, CancellationToken cancellationToken);
}

internal class ServiceFactoryAdapter<TContainerBuilder>
    (Func<IServiceCollection, CancellationToken, ValueTask<IServiceProviderFactory<TContainerBuilder>>> factory) : IServiceFactoryAdapter
    where TContainerBuilder : notnull
{
    private IServiceProviderFactory<TContainerBuilder>? _serviceProviderFactory;

    public async ValueTask<object> CreateBuilder(IServiceCollection services, CancellationToken cancellationToken)
    {
        _serviceProviderFactory = await factory(services, cancellationToken);
        return _serviceProviderFactory.CreateBuilder(services);
    }

    #if NET6_0_OR_GREATER
    public ValueTask<IServiceProvider> CreateServiceProvider(object containerBuilder, CancellationToken cancellationToken)
    {
        if (_serviceProviderFactory is null) throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
        return ValueTask.FromResult(_serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder));
    }
    #else
    public async ValueTask<IServiceProvider> CreateServiceProvider(object containerBuilder, CancellationToken cancellationToken)
    {
        if (_serviceProviderFactory is null) throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
        await Task.Yield();
        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
    #endif
}
