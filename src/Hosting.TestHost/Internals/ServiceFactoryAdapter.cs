using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Hosting.Internals;

internal class ServiceFactoryAdapter<TContainerBuilder> : IServiceFactoryAdapter where TContainerBuilder : notnull
{
    private IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory = null!;
    private readonly Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> _factoryResolver = null!;

    public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
    {
        _serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
    }

    public ServiceFactoryAdapter(
        Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver
    )
    {
        _factoryResolver = factoryResolver ?? throw new ArgumentNullException(nameof(factoryResolver));
    }

    public object CreateBuilder(IServiceCollection services, HostBuilderContext hostBuilderContext)
    {
        if (_serviceProviderFactory == null!)
        {
            _serviceProviderFactory = _factoryResolver(hostBuilderContext);

            if (_serviceProviderFactory == null)
            {
                throw new InvalidOperationException("The resolver returned a null IServiceProviderFactory");
            }
        }

        return _serviceProviderFactory.CreateBuilder(services);
    }

    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        if (_serviceProviderFactory == null)
        {
            throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
        }

        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
