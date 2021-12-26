using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

class ConventionTypeRegistrar : ITypeRegistrar
{
    private readonly IConventionContext _conventionContext;
    private readonly ServiceCollection _services;

    public ConventionTypeRegistrar(IConventionContext conventionContext)
    {
        _conventionContext = conventionContext;
        _services = new ServiceCollection();
    }

    public void Register(Type service, Type implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        _services.AddSingleton(service, _ => factory());
    }

    public ITypeResolver Build()
    {
        _services.AddSingleton(_conventionContext.Get<IConfiguration>());
        var factory = _conventionContext.GetOrAdd<ICommandAppServiceProviderFactory>(() => new DefaultServiceProviderFactory());
        return new ConventionTypeResolver(factory.CreateServiceProvider(_services, _conventionContext));
    }
}
