using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class DependencyContextAssemblyCandidateFinderTests : AutoTestBase
    {
        public DependencyContextAssemblyCandidateFinderTests(ITestOutputHelper outputHelper) : base(outputHelper){}

        [Fact]
        public void FindsAssembliesInCandidates_Params()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly), Logger);
            var items = resolver.GetCandidateAssemblies("Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions")
                .Select(x => x.GetName().Name)
                .ToArray();

            foreach (var item in items) Logger.LogInformation(item);
            items
                .Should()
                .Contain("Rocket.Surgery.Conventions.Tests");
        }

        [Fact]
        public void FindsAssembliesInCandidates_Enumerable()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly), Logger);
            var items = resolver.GetCandidateAssemblies(new[] { "Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions" }.AsEnumerable())
                .Select(x => x.GetName().Name)
                .ToArray();

            foreach (var item in items) Logger.LogInformation(item);
            items
                .Should()
                .Contain("Rocket.Surgery.Conventions.Tests");
        }

        [Fact]
        public void FindsAssembliesInCandidates_Empty()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly), Logger);
            var items = resolver.GetCandidateAssemblies(new string[] {  }.AsEnumerable())
                .Select(x => x.GetName().Name)
                .ToArray();

            foreach (var item in items) Logger.LogInformation(item);
            items.Should().BeEmpty();
        }
    }
}
