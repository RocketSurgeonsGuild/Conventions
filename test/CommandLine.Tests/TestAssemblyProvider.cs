using System.Reflection;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.CommandLine.Tests;

internal class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(CommandLineContext).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}
