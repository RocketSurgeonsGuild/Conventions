using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Tests;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

[assembly: Contribution(typeof(AssemblyCandidateResolverTests.Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    public class AssemblyCandidateResolverTests
    {
        internal class Contrib : IServiceConvention{
            public void Register(ServiceConventionContext context)
            {
            }
        }

        [Fact]
        public void FindsAssembliesInCandidates_Params()
        {
            var resolver = new AssemblyCandidateFinder(DependencyContext.Load(typeof(AssemblyCandidateResolverTests).GetTypeInfo().Assembly), A.Fake<ILogger>());
            var items = resolver.GetCandidateAssemblies("Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions")
                .Select(x => x.GetName().Name);
            items
                .Should()
                .Contain("Rocket.Surgery.Conventions.Tests");
        }

        [Fact]
        public void FindsAssembliesInCandidates_Enumerable()
        {
            var resolver = new AssemblyCandidateFinder(DependencyContext.Load(typeof(AssemblyCandidateResolverTests).GetTypeInfo().Assembly), A.Fake<ILogger>());
            var items = resolver.GetCandidateAssemblies(new[] { "Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions" }.AsEnumerable())
                .Select(x => x.GetName().Name);
            items
                .Should()
                .Contain("Rocket.Surgery.Conventions.Tests");
        }
    }
}
