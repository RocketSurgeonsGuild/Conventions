using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

internal class DryIocConventionServiceProviderFactory(IConventionContext conventionContext, IContainer? container = null) : IServiceProviderFactory<IContainer>
{
    public IContainer CreateBuilder(IServiceCollection services)
    {
#pragma warning disable CA2000
        var container1 = container ?? new Container().WithDependencyInjectionAdapter();
#pragma warning restore CA2000
        container1 = container1.ApplyConventions(conventionContext, services);
        container1.Populate(services);
        return container1;
    }

    public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
    {
        return conventionContext.GetOrAdd(() => new DryIocOptions()).NoMoreRegistrationAllowed
            ? containerBuilder.WithNoMoreRegistrationAllowed()
            : containerBuilder;
    }
}
