using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.DryIoc;

internal class DryIocConventionServiceProviderFactory(IClavusContext context, IServiceCollection services, IContainer container) : IServiceProviderFactory<IContainer>
{
    public IContainer CreateBuilder(IServiceCollection _) => container;
    public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
    {
        var provider = containerBuilder.WithDependencyInjectionAdapter(services);
        if (context.DryIocOptions.NoMoreRegistrationAllowed)
        {
            provider.Container.WithNoMoreRegistrationAllowed();
        }

        return provider;
    }
}
