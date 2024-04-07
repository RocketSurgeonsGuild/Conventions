using System.Reflection;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

internal sealed  class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(DryIocConventionServiceProviderFactory).GetTypeInfo().Assembly,
            typeof(RocketHostExtensions).GetTypeInfo().Assembly,
            #if NET8_0_OR_GREATER
            typeof(RocketHostApplicationExtensions).GetTypeInfo().Assembly,
            #endif
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}
