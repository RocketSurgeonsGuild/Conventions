using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.DryIoc;

internal class DryIocConventionServiceProviderFactory(IClavusContext context, IContainer container) : IServiceProviderFactory<IContainer>
{
    public IContainer CreateBuilder(IServiceCollection services)
    {
        var container1 = container;
        container1.Populate(services);
        return container1;
    }

    public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
    {
        return ( context.GetOrAdd(() => new DryIocOptions()).NoMoreRegistrationAllowed
            ? containerBuilder.WithNoMoreRegistrationAllowed()
            : containerBuilder ).WithDependencyInjectionAdapter();
    }
}
