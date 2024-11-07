using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

internal class ConventionTypeRegistrar : ITypeRegistrar, IServiceProvider
{
    private readonly IServiceCollection _services = new ServiceCollection();

    private IServiceProvider? _rootServiceProvider;
    private ServiceProvider? _internalServices;

    [MemberNotNull(nameof(_rootServiceProvider))]
    internal void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _rootServiceProvider = serviceProvider;
    }

    internal object? GetService(Type type)
    {
        try
        {
            return _internalServices?.GetService(type) ?? _rootServiceProvider?.GetService(type);
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
        // ReSharper disable once NullableWarningSuppressionIsUsed
        _services.AddSingleton(service, _ => ActivatorUtilities.GetServiceOrCreateInstance(_rootServiceProvider!, implementation));
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
        #pragma warning disable CS8604 // Possible null reference argument.
        return new ConventionTypeResolver(_rootServiceProvider, _internalServices);
        #pragma warning restore CS8604 // Possible null reference argument.
    }
}
