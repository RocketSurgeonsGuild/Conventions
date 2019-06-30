using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class RocketEnvironmentTests : AutoTestBase
    {
        public RocketEnvironmentTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void ShouldCreateAnEnvironmentAsExpected()
        {
            var env = new RocketEnvironment("someenv", "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            env.EnvironmentName.Should().Be("someenv");
            env.ApplicationName.Should().Be("appname");
            env.IsEnvironment("someenv").Should().BeTrue();
        }

        [Fact]
        public void CheckForDevelopmentShouldFailOnNull()
        {
            var env = new RocketEnvironment(RocketEnvironments.Development, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            Action a = () => RocketEnvironmentExtensions.IsDevelopment(null);
            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CheckForStagingShouldFailOnNull()
        {
            var env = new RocketEnvironment(RocketEnvironments.Development, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            Action a = () => RocketEnvironmentExtensions.IsEnvironment(null, RocketEnvironments.Staging);
            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CheckForProductionShouldFailOnNull()
        {
            var env = new RocketEnvironment(RocketEnvironments.Development, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            Action a = () => RocketEnvironmentExtensions.IsEnvironment(null, RocketEnvironments.Production);
            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CheckForEnvironmentShouldFailOnNull()
        {
            var env = new RocketEnvironment(RocketEnvironments.Development, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            Action a = () => RocketEnvironmentExtensions.IsEnvironment(null, RocketEnvironments.Development);
            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldCheckForDevelopment()
        {
            var env = new RocketEnvironment(RocketEnvironments.Development, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            env.IsDevelopment().Should().BeTrue();
        }

        [Fact]
        public void ShouldCheckForStaging()
        {
            var env = new RocketEnvironment(RocketEnvironments.Staging, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            env.IsStaging().Should().BeTrue();
        }

        [Fact]
        public void ShouldCheckForProduction()
        {
            var env = new RocketEnvironment(RocketEnvironments.Production, "appname", Directory.GetCurrentDirectory(),
                AutoFake.Resolve<IFileProvider>());

            env.IsProduction().Should().BeTrue();
        }

        [Theory]
        [InlineData(RocketEnvironments.Development)]
        [InlineData(RocketEnvironments.Staging)]
        [InlineData(RocketEnvironments.Production)]
        public void ShouldConvertFromIHostingEnvironment(string envName)
        {
            var fakeEnvironment = A.Fake<IHostingEnvironment>();
            A.CallTo(() => fakeEnvironment.EnvironmentName).Returns(envName);
            var env = fakeEnvironment.Convert();

            env.EnvironmentName.Should().Be(envName);
        }

#if NETCOREAPP3_0
        [Theory]
        [InlineData(RocketEnvironments.Development)]
        [InlineData(RocketEnvironments.Staging)]
        [InlineData(RocketEnvironments.Production)]
        public void ShouldConvertFromIHostEnvironment(string envName)
        {
            var fakeEnvironment = A.Fake<IHostEnvironment>();
            A.CallTo(() => fakeEnvironment.EnvironmentName).Returns(envName);
            var env = fakeEnvironment.Convert();

            env.EnvironmentName.Should().Be(envName);
        }
#endif
    }
}