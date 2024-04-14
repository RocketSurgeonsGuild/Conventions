using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Adapters;

namespace Rocket.Surgery.Conventions;

internal class ConventionServiceProviderFactory : IServiceFactoryAdapter, IServiceProviderFactory<object>
{
    private readonly IConventionContext _conventionContext;
    private readonly IServiceFactoryAdapter? _innerServiceProviderFactory;
    private readonly bool _applyDefaultConventions;

    public static IServiceFactoryAdapter From(IConventionContext conventionContext, bool applyDefaultConventions = true)
    {
        var factoryDelegate = conventionContext.Get<Func<IConventionContext, IServiceFactoryAdapter>>();
        return new ConventionServiceProviderFactory(conventionContext, factoryDelegate?.Invoke(conventionContext), applyDefaultConventions);
    }

    public static IServiceProviderFactory<object> Wrap(IConventionContext conventionContext, bool applyDefaultConventions = true)
    {
        var factoryDelegate = conventionContext.Get<Func<IConventionContext, IServiceFactoryAdapter>>();
        return new ConventionServiceProviderFactory(conventionContext, factoryDelegate?.Invoke(conventionContext), applyDefaultConventions);
    }

    private ConventionServiceProviderFactory(
        IConventionContext conventionContext, IServiceFactoryAdapter? innerServiceProviderFactory, bool applyDefaultConventions
    )
    {
        _conventionContext = conventionContext;
        _innerServiceProviderFactory = innerServiceProviderFactory;
        _applyDefaultConventions = applyDefaultConventions;
    }

    public object CreateBuilder(IServiceCollection services)
    {
        if (_applyDefaultConventions)
        {
            services.ApplyConventions(_conventionContext);
            new LoggingBuilder(services).ApplyConventions(_conventionContext);
        }

        return _innerServiceProviderFactory?.CreateBuilder(services) ?? services;
    }

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

internal class LazyConventionServiceProviderFactory : IServiceFactoryAdapter, IServiceProviderFactory<object>
{
    private readonly Lazy<IServiceProviderFactory<object>> _factory;

    public static IServiceProviderFactory<object> Create(Func<IServiceProviderFactory<object>> factory)
    {
        return new LazyConventionServiceProviderFactory(factory);
    }

    private LazyConventionServiceProviderFactory(Func<IServiceProviderFactory<object>> factory)
    {
        _factory = new Lazy<IServiceProviderFactory<object>>(factory);
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
