using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

class ConventionTypeResolver : ITypeResolver
{
    private readonly IServiceProvider _serviceProvider;

    public ConventionTypeResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? Resolve(Type? type)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type!);
    }
}