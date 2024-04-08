﻿using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Sample.DependencyThree;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class DefaultAssemblyCandidateFinderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper, LogLevel.Trace)
{
    [Fact]
    public void FindsAssembliesInCandidates_Params()
    {
        var resolver = new DefaultAssemblyProvider(
            new[]
            {
                typeof(DefaultAssemblyCandidateFinderTests).GetTypeInfo().Assembly,
                typeof(Class3).GetTypeInfo().Assembly
            },
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
                    "Sample.DependencyThree",
                    "Sample.DependencyTwo",
                    "Rocket.Surgery.Conventions.Tests"
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
        var resolver = new DefaultAssemblyProvider(
            new[]
            {
                typeof(DefaultAssemblyCandidateFinderTests).GetTypeInfo().Assembly,
                typeof(Class3).GetTypeInfo().Assembly
            },
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
                    "Sample.DependencyThree",
                    "Sample.DependencyTwo",
                    "Rocket.Surgery.Conventions.Tests"
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
        var resolver = new DefaultAssemblyProvider(new[] { typeof(DefaultAssemblyCandidateFinderTests).GetTypeInfo().Assembly }, Logger);
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
