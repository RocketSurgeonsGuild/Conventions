using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests
{
    public class RocketHostBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Should_UseAppDomain()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAppDomain(AppDomain.CurrentDomain));

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseAssemblies()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseRocketBooster()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain));

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDiagnosticLogging()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain), x => x.UseDiagnosticLogging(c => c.AddSerilog()));

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDependencyContext()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAppDomain(AppDomain.CurrentDomain));

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_ConfigureServices()
        {
            var convention = A.Fake<ServiceConvention>();
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAppDomain(AppDomain.CurrentDomain)
                       .ConfigureServices(convention)
                );

            builder.Build();
            A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IServiceCollection>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureConfiguration()
        {
            var convention = A.Fake<ConfigurationConvention>();
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAppDomain(AppDomain.CurrentDomain).ConfigureConfiguration(convention));

            builder.Build();
            A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IConfigurationBuilder>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureHosting()
        {
            var convention = A.Fake<WebAssemblyHostingConvention>();
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAppDomain(AppDomain.CurrentDomain).ConfigureHosting(convention));

            builder.Build();
            A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IWebAssemblyHostBuilder>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureLogging()
        {
            var convention = A.Fake<LoggingConvention>();
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(rb => rb.UseAppDomain(AppDomain.CurrentDomain).ConfigureLogging(convention));

            builder.Build();
            A.CallTo(() => convention.Invoke(A<IConventionContext>._,  A<IConfiguration>._, A<ILoggingBuilder>._)).MustHaveHappened();
        }


        [Fact]
        public void Should_Build_The_Host_Correctly()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault().ConfigureRocketSurgery(AppDomain.CurrentDomain);

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}