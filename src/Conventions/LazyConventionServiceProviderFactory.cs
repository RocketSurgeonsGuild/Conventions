using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Adapters;

namespace Rocket.Surgery.Conventions;

internal class LazyConventionServiceProviderFactory(Func<IServiceProviderFactory<object>> factory) : IServiceFactoryAdapter, IServiceProviderFactory<object>
{
    private readonly Lazy<IServiceProviderFactory<object>> _factory = new(factory);

    public static IServiceProviderFactory<object> Create(Func<IServiceProviderFactory<object>> factory)
    {
        return new LazyConventionServiceProviderFactory(factory);
    }

    public object CreateBuilder(IServiceCollection services)
    {
        return _factory.Value.CreateBuilder(services);
    }

    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        return _factory.Value.CreateServiceProvider(containerBuilder);
    }
}