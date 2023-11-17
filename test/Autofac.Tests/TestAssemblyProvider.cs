using System.Reflection;
using Rocket.Surgery.Conventions.Autofac;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.Web.Hosting;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

internal sealed class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(AutofacConventionServiceProviderFactory).GetTypeInfo().Assembly,
            typeof(RocketHostExtensions).GetTypeInfo().Assembly,
            typeof(RocketWebHostExtensions).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}
