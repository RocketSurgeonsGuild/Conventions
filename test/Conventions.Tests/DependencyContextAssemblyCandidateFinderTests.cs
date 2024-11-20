using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class DependencyContextAssemblyCandidateFinderTests(ITestOutputHelper outputHelper)  : AutoFakeTest<LocalTestContext>(LocalTestContext.Create(outputHelper))
{
    [field: AllowNull, MaybeNull]
    private ILoggerFactory LoggerFactory => field ??= CreateLoggerFactory();
    private ILogger Logger => LoggerFactory.CreateLogger(GetType());

    [Fact]
    public void FindsAssembliesInCandidates_Params()
    {
        var resolver = new DependencyContextAssemblyProvider(
            DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly)!,
            Logger
        );
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
                    "Sample.DependencyTwo",
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
    public void FindsAssembliesInCandidates_Enumerable()
    {
        var resolver = new DependencyContextAssemblyProvider(
            DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly)!,
            Logger
        );
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
                    "Sample.DependencyTwo",
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
    public void FindsAssembliesInCandidates_Params_Multiples()
    {
        var resolver = new DependencyContextAssemblyProvider(
            DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly)!,
            Logger
        );
        var items = resolver
                   .GetAssemblies(z => z.FromAssemblyDependenciesOf<IConventionContext>().FromAssemblyDependenciesOf<ConventionContext>())
                   .Select(x => x.GetName().Name)
                   .ToArray();
        var items2 = resolver
                    .GetAssemblies(z => z.FromAssemblyDependenciesOf<IConventionContext>().FromAssemblyDependenciesOf<ConventionContext>())
                    .Select(x => x.GetName().Name)
                    .ToArray();

        foreach (var item in items)
        {
            Logger.LogInformation(item);
        }

        foreach (var item in items2)
        {
            Logger.LogInformation(item);
        }

        items
           .Should()
           .Contain(
                new[]
                {
                    "Sample.DependencyOne",
                    "Sample.DependencyTwo",
                    "Sample.DependencyThree",
                    "Rocket.Surgery.Conventions.Tests",
                }
            );
        items
           .Last()
           .Should()
           .Be("Rocket.Surgery.Conventions.Tests");
        items.Should().BeEquivalentTo(items2);
    }

    [Fact]
    public void FindsAssembliesInCandidates_Empty()
    {
        var resolver = new DependencyContextAssemblyProvider(
            DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly)!,
            Logger
        );
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
