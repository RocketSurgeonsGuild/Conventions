using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

internal class DryIocConventionServiceProviderFactory(IConventionContext context, IContainer container) : IServiceProviderFactory<IContainer>
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