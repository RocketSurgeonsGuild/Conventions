using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class AppDomainAssemblyCandidateFinderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public void FindsAssembliesInCandidates_Params()
    {
        var resolver = new AppDomainAssemblyProvider(AppDomain.CurrentDomain, Logger);
        var items = resolver
                   .GetAssemblies(z => z.FromAssemblyDependenciesOf<IConventionContext>().FromAssemblyDependenciesOf<ConventionContext>())
                   .Select(x => x.GetName().Name)
                   .ToArray();

        foreach (var item in items)
        {
            Logger.LogInformation(item);
        }

        items
           .Should()
           .Contain(
                new[]
                {
                    "Sample.DependencyOne",
                    //"Sample.DependencyTwo",
                    "Sample.DependencyThree",
                    "Rocket.Surgery.Conventions.Tests",
                }
            );
        items
           .Last()
           .Should()
           .Be("Rocket.Surgery.Conventions.Tests");
    }

    [Fact]
    public void FindsAssembliesInCandidates_Empty()
    {
        var resolver = new AppDomainAssemblyProvider(AppDomain.CurrentDomain, Logger);
        var items = resolver
                   .GetAssemblies(z => { })
                   .Select(x => x.GetName().Name)
                   .ToArray();

        foreach (var item in items)
        {
            Logger.LogInformation(item);
        }

        items.Should().BeEmpty();
    }
}