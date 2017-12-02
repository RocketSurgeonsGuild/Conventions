#if !NETCOREAPP1_1
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Sample.DependencyThree;
using Sample.DependencyTwo;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class DefaultAssemblyCandidateFinderTests : AutoTestBase
    {
        public DefaultAssemblyCandidateFinderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public void FindsAssembliesInCandidates_Params()
        {
            var resolver = new DefaultAssemblyCandidateFinder(new[]
            {
                typeof(DefaultAssemblyCandidateFinderTests).GetTypeInfo().Assembly,
                typeof(Class3).GetTypeInfo().Assembly
            }, Logger);
            var items = resolver.GetCandidateAssemblies("Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions")
                .Select(x => x.GetName().Name)
                .ToArray();

            foreach (var item in items) Logger.LogInformation(item);

            items
                .Should()
                .Contain(new[] {
                    "Sample.DependencyOne",
                    "Sample.DependencyThree",
                    "Rocket.Surgery.Conventions.Tests",
                });
            items
                .Should()
                .NotContain("Sample.DependencyTwo");
            items
                .Last()
                .Should()
                .Be("Rocket.Surgery.Conventions.Tests");
        }

        [Fact]
        public void FindsAssembliesInCandidates_Params_Multiples()
        {
            var resolver = new DefaultAssemblyCandidateFinder(new[]
            {
                typeof(DefaultAssemblyCandidateFinderTests).GetTypeInfo().Assembly,
                typeof(Class3).GetTypeInfo().Assembly
            }, Logger);
            var items = resolver.GetCandidateAssemblies(new[] { "Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions" })
                .Select(x => x.GetName().Name)
                .ToArray();
            var items2 = resolver.GetCandidateAssemblies("Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions")
                .Select(x => x.GetName().Name)
                .ToArray();

            foreach (var item in items) Logger.LogInformation(item);
            foreach (var item in items2) Logger.LogInformation(item);

            items
                .Should()
                .Contain(new[] {
                    "Sample.DependencyOne",
                    "Sample.DependencyThree",
                    "Rocket.Surgery.Conventions.Tests",
                });
            items
                .Should()
                .NotContain("Sample.DependencyTwo");
            items
                .Last()
                .Should()
                .Be("Rocket.Surgery.Conventions.Tests");
            items.Should().BeEquivalentTo(items2);
        }

        [Fact]
        public void FindsAssembliesInCandidates_Empty()
        {
            var resolver = new DefaultAssemblyCandidateFinder(new[] { typeof(DefaultAssemblyCandidateFinderTests).GetTypeInfo().Assembly }, Logger);
            var items = resolver.GetCandidateAssemblies(new string[] { }.AsEnumerable())
                .Select(x => x.GetName().Name)
                .ToArray();

            foreach (var item in items) Logger.LogInformation(item);
            items.Should().BeEmpty();
        }
    }
}
#endif
