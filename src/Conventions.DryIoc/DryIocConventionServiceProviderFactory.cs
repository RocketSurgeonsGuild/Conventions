using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

internal class DryIocConventionServiceProviderFactory : IServiceProviderFactory<IContainer>
{
    private readonly IConventionContext _conventionContext;
    private readonly IContainer? _container;

    public DryIocConventionServiceProviderFactory(IConventionContext conventionContext, IContainer? container = null)
    {
        _conventionContext = conventionContext;
        _container = container;
    }

    public IContainer CreateBuilder(IServiceCollection services)
    {
#pragma warning disable CA2000
        var container = _container ?? new Container().WithDependencyInjectionAdapter();
#pragma warning restore CA2000
        container = container.ApplyConventions(_conventionContext, services);
        container.Populate(services);
        return container;
    }

    public IServiceProvider CreateServiceProvider(IContainer containerBuilder)
    {
        return _conventionContext.GetOrAdd(() => new DryIocOptions()).NoMoreRegistrationAllowed
            ? containerBuilder.WithNoMoreRegistrationAllowed()
            : containerBuilder;
    }
}
