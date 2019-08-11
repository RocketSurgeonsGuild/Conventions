using System;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.Configuration.Tests
{
    public class ConfigurationBuilderTests : AutoTestBase
    {
        public ConfigurationBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public void Constructs()
        {
            var configuration = AutoFake.Resolve<IConfiguration>();
            var builder = AutoFake.Resolve<ConfigurationBuilder>(new TypedParameter(typeof(ILogger), Logger));

            builder.Logger.Should().NotBeNull();
            builder.Configuration.Should().NotBeNull();

            builder.Configuration.Should().BeSameAs(configuration);
            Action a = () => { builder.AppendConvention(A.Fake<IConfigurationConvention>()); };
            a.Should().NotThrow();
            a = () => { builder.AppendDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void IsAMsftConfigurationBuilder()
        {
            var configuration = AutoFake.Resolve<IConfiguration>();
            var builder = AutoFake.Resolve<ConfigurationBuilder>();

            var msftBuilder = builder as Microsoft.Extensions.Configuration.IConfigurationBuilder;
            msftBuilder.Add(A.Fake<IConfigurationSource>());
            msftBuilder.Build();

            msftBuilder.Properties.Should().NotBeNull();
            msftBuilder.Sources.Should().NotBeNull();

            A.CallTo(() =>
                AutoFake.Resolve<Microsoft.Extensions.Configuration.IConfigurationBuilder>()
                    .Add(A<IConfigurationSource>._)).MustHaveHappened();
            A.CallTo(() =>
                AutoFake.Resolve<Microsoft.Extensions.Configuration.IConfigurationBuilder>()
                    .Build()).MustHaveHappened();
        }

        [Fact]
        public void BuildsSafely()
        {
            var builder = AutoFake.Resolve<ConfigurationBuilder>();

            Action a = () => builder.Build();
            a.Should().NotThrow();
        }
    }
}
