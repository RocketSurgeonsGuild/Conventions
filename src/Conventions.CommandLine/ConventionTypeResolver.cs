using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class ConventionTypeResolver : ITypeResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _instances;

    public ConventionTypeResolver(IServiceProvider serviceProvider, IServiceProvider instances)
    {
        _serviceProvider = serviceProvider;
        _instances = instances;
    }

    public object? Resolve(
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        Type? type
    )
    {
        return _serviceProvider.GetService(type)
            ?? _instances.GetService(type)
            ?? ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type!);
    }
}
