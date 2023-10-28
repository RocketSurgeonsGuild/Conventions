using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Tests;

internal sealed class TestAssemblyProvider : IAssemblyProvider, IAssemblyCandidateFinder
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(ConventionContextBuilder).GetTypeInfo().Assembly,
            typeof(IConventionContext).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }

    public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
    {
        return GetAssemblies();
    }
}
