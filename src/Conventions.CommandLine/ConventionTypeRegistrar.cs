using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConventionTypeRegistrar : ITypeRegistrar, IServiceProvider
{
    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    private IServiceProvider _serviceProvider = null!;
    private readonly IServiceCollection _services;
    private ServiceProvider? _internalServices;

    public ConventionTypeRegistrar()
    {
        _services = new ServiceCollection();
    }

    public void Register(
        Type service,
        Type implementation
    )
    {
#pragma warning disable IL2067
        _services.AddSingleton(service, implementation);
#pragma warning restore IL2067
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
