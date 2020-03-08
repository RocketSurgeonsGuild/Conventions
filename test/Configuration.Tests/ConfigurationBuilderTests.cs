using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;
using ConfigurationBuilder = Rocket.Surgery.Conventions.Configuration.ConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration.Tests
{
    public class ConfigurationBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Constructs()
        {
            var configuration = AutoFake.Resolve<IConfiguration>();
            var builder = AutoFake.Resolve<ConfigurationBuilder>();

            builder.Logger.Should().NotBeNull();
            builder.Configuration.Should().NotBeNull();

            builder.Configuration.Should().BeSameAs(configuration);
            Action a = () => { builder.AppendConvention(A.Fake<IConfigurationConvention>()); };
            a.Should().NotThrow();
            a = () => { builder.AppendDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void BuildsSafely()
        {
            var builder = AutoFake.Resolve<ConfigurationBuilder>();

            Action a = () => builder.Build();
            a.Should().NotThrow();
        }

        public ConfigurationBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}