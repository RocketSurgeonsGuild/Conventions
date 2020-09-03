using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyModel;
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
        public void Should_Call_Through_To_Delegate_Methods()
        {
            AutoFake.Provide(Array.Empty<string>());
            LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .PrependDelegate(new Action(() => { }))
                       .AppendDelegate(new Action(() => { }))
                );
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void Should_Call_Through_To_Convention_Methods()
        {
            AutoFake.Provide(Array.Empty<string>());
            var convention = AutoFake.Resolve<IConvention>();
            LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .PrependConvention(convention)
                       .AppendConvention(convention)
                );
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void Should_UseAppDomain()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseAppDomain(AppDomain.CurrentDomain)
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseAssemblies()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                );

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
               .UseRocketBooster(
                    RocketBooster.For(AppDomain.CurrentDomain),
                    x => x.UseDiagnosticLogging(c => c.AddSerilog())
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDependencyContext()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_ConfigureServices()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureServices(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(ServiceConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureConfiguration()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureConfiguration(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(ConfigConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureLogging()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureLogging(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(LoggingConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_Build_The_Host_Correctly()
        {
            var serviceConventionFake = A.Fake<IServiceConvention>();
            var configurationConventionFake = A.Fake<IConfigConvention>();

            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(
                            new BasicConventionScanner(
                                A.Fake<IServiceProviderDictionary>(),
                                serviceConventionFake,
                                configurationConventionFake
                            )
                        )
                       .UseAssemblyCandidateFinder(
                            new DefaultAssemblyCandidateFinder(new[] { typeof(RocketHostBuilderTests).Assembly })
                        )
                       .UseAssemblyProvider(
                            new DefaultAssemblyProvider(new[] { typeof(RocketHostBuilderTests).Assembly })
                        )
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}