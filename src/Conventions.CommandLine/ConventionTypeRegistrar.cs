using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConventionTypeRegistrar : ITypeRegistrar, IServiceProvider
{
    private readonly IServiceCollection _services;

    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    private IServiceProvider _serviceProvider = null!;
    private ServiceProvider? _internalServices;

    public ConventionTypeRegistrar()
    {
        _services = new ServiceCollection();
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

    public void Register(
        Type service,
        #pragma warning disable IL2092
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type implementation
        #pragma warning restore IL2092
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

    public ITypeResolver Build()
    {
        _internalServices = _services.BuildServiceProvider();
        return new ConventionTypeResolver(_serviceProvider, this);
    }
}