using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

internal class ConventionTypeResolver(IServiceProvider serviceProvider, IServiceProvider instances) : ITypeResolver
{
    #pragma warning disable IL2092
    public object? Resolve([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type? type)
    #pragma warning restore IL2092
    {
        if (type is null) return null;
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return serviceProvider.GetService(type!)
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
         ?? instances.GetService(type!)
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
         ?? ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type!);
    }
}
