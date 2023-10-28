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

    public object? Resolve(Type? type)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return _serviceProvider.GetService(type!)
               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            ?? _instances.GetService(type!)
#pragma warning disable IL2067
               // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            ?? ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type!);
#pragma warning restore IL2067
    }
}
