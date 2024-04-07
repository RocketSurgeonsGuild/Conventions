using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Adapters;
using ServiceFactoryAdapter = System.Func<Rocket.Surgery.Conventions.IConventionContext, System.Threading.CancellationToken, System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.Adapters.IServiceFactoryAdapter>>;

namespace Rocket.Surgery.Conventions;

internal class ConventionServiceProviderFactory : IServiceFactoryAdapter, IServiceProviderFactory<object>
{
    public static async ValueTask<IServiceFactoryAdapter> FromAsync(IConventionContext conventionContext, CancellationToken cancellationToken)
    {
        var factoryDelegate = conventionContext.Get<ServiceFactoryAdapter>();
        return new ConventionServiceProviderFactory(conventionContext, await factoryDelegate!.Invoke(conventionContext, cancellationToken));
    }

    public static async ValueTask<IServiceProviderFactory<object>> WrapAsync(IConventionContext conventionContext, CancellationToken cancellationToken)
    {
        var factoryDelegate = conventionContext.Get<ServiceFactoryAdapter>();
        return new ConventionServiceProviderFactory(conventionContext, await factoryDelegate!.Invoke(conventionContext, cancellationToken));
    }

    private readonly IConventionContext _conventionContext;
    private readonly IServiceFactoryAdapter? _innerServiceProviderFactory;

    private ConventionServiceProviderFactory(IConventionContext conventionContext, IServiceFactoryAdapter? innerServiceProviderFactory)
    {
        _conventionContext = conventionContext;
        _innerServiceProviderFactory = innerServiceProviderFactory;
    }

    public object CreateBuilder(IServiceCollection services) => _innerServiceProviderFactory?.CreateBuilder(services) ?? services;

    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        if (_innerServiceProviderFactory != null) return _innerServiceProviderFactory.CreateServiceProvider(containerBuilder);
        if (containerBuilder is IServiceCollection services)
        {
            return services.BuildServiceProvider(_conventionContext.GetOrAdd(() => new ServiceProviderOptions()));
        }

        throw new NotSupportedException("Could not create service provider from provided container builder");
    }
}
