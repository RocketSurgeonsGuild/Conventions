using System.Reflection;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.CommandLine.Tests;

internal sealed class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(CommandLineConvention).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}
