using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConventionTypeRegistrar : ITypeRegistrar, IServiceProvider
{
    private IServiceProvider _serviceProvider;
    private readonly IServiceCollection _services;
    private Dictionary<Type, object> _instances = new();
    private ServiceProvider? _internalServices;

    public ConventionTypeRegistrar()
    {
        _services = new ServiceCollection();
    }

    public void Register(
        Type service,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        Type implementation
    )
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

    internal void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal object? GetService(Type type)
    {
        try
        {
            return _internalServices?.GetService(type);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    object? IServiceProvider.GetService(Type type)
    {
        return GetService(type);
    }

    public ITypeResolver Build()
    {
        _internalServices = _services.BuildServiceProvider();
        return new ConventionTypeResolver(_serviceProvider, this);
    }
}
