using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConventionTypeRegistrar : ITypeRegistrar
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

        var factory = ConventionServiceProviderFactory.From(_conventionContext);
        var provider = factory.CreateServiceProvider(factory.CreateBuilder(_services));
        return new ConventionTypeResolver(provider);
    }
}
