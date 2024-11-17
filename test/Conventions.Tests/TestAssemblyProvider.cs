using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Tests;

internal sealed class TestAssemblyProvider : ICompiledTypeProvider
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
        Action<IReflectionAssemblySelector> action,
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

    public IServiceCollection Scan(IServiceCollection services, Action<IServiceDescriptorAssemblySelector> selector, int lineNumber = 0, string filePath = "", string argumentExpression = "") => throw new NotImplementedException();

    public IEnumerable<Type> GetTypes(
        Func<IReflectionTypeSelector, IEnumerable<Type>> selector,
        int lineNumber = 0,
        string filePath = "",
        string argumentExpression = ""
    )
    {
        return selector(new TypeProviderAssemblySelector());
    }
}
