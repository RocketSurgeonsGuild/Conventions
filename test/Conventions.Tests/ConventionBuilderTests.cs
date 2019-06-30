using System.Collections.Generic;
using FluentAssertions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionBuilderTests : AutoTestBase
    {
        public ConventionBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private class CCBuilder : ConventionBuilder<CCBuilder, IServiceConvention, ServiceConventionDelegate>
        {
            public CCBuilder(IConventionScanner scanner, IAssemblyProvider assemblyProvider, IAssemblyCandidateFinder assemblyCandidateFinder, IDictionary<object, object> properties) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
            {
            }
        }

        [Fact]
        public void ShouldConstruct()
        {
            var properties = new Dictionary<object, object>();
            AutoFake.Provide<IDictionary<object, object>>(properties);
            var builder = AutoFake.Resolve<CCBuilder>();

            builder.Should().NotBeNull();
            builder.Properties.Should().NotBeNull();
            builder.Scanner.Should().NotBeNull();
            builder.AssemblyProvider.Should().NotBeNull();
            builder.AssemblyCandidateFinder.Should().NotBeNull();

            builder["a"] = "b";

            builder["a"].Should().Be("b");
        }
    }
}