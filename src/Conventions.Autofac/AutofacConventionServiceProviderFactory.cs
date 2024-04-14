using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

internal class AutofacConventionServiceProviderFactory(ContainerBuilder? container = null) : IServiceProviderFactory<ContainerBuilder>
{
    private readonly ContainerBuilder _container = container ?? new ContainerBuilder();

    public ContainerBuilder CreateBuilder(IServiceCollection services)
    {
        _container.Populate(services);
        return _container;
    }

    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
    {
        return containerBuilder.Build().Resolve<IServiceProvider>();
    }
}