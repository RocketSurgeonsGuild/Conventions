using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Tests;

internal sealed class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(ConventionContextBuilder).GetTypeInfo().Assembly,
            typeof(IConventionContext).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly,
        };
    }

    public IEnumerable<Assembly> GetAssemblies(
        Action<IAssemblyProviderAssemblySelector> action,
        string filePath = "",
        string memberName = "",
        int lineNumber = 0
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
        string filePath = "",
        string memberName = "",
        int lineNumber = 0
    )
    {
        return selector(new TypeProviderAssemblySelector());
    }
}