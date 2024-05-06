using System.Reflection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DryIoc;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

internal sealed class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(DryIocConventionServiceProviderFactory).GetTypeInfo().Assembly,
            typeof(RocketHostApplicationExtensions).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
        };
    }

    public IEnumerable<Assembly> GetAssemblies(
        Action<IAssemblyProviderAssemblySelector> action,
        int lineNumber = 0,
        string filePath = "",
        string argumentExpression = ""
    )
    {
        var selector = new AssemblyProviderAssemblySelector();
        action(selector);

        return selector.AssemblyDependencies.Any()
            ? GetAssemblies()
            : selector.Assemblies;
    }

    public IEnumerable<Type> GetTypes(
        Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector,
        int lineNumber = 0,
        string filePath = "",
        string argumentExpression = ""
    )
    {
        return selector(new TypeProviderAssemblySelector());
    }
}