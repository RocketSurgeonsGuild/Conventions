using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Extensions.CommandLine.Tests;

internal sealed class TestAssemblyProvider : ICompiledTypeProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(CommandLineConvention).GetTypeInfo().Assembly,
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

    public IEnumerable<Type> GetTypes(
        Func<IReflectionTypeSelector, IEnumerable<Type>> selector,
        int lineNumber = 0,
        string filePath = "",
        string argumentExpression = ""
    )
    {
        return selector(new TypeProviderAssemblySelector());
    }

    public IServiceCollection Scan(IServiceCollection services, Action<IServiceDescriptorAssemblySelector> selector, int lineNumber = 0, string filePath = "", string argumentExpression = "") => throw new NotImplementedException();
}
