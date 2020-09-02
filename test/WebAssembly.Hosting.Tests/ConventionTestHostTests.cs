using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.WebAssembly.Hosting;
using Xunit;
using Xunit.Abstractions;
using IConventionScanner = Rocket.Surgery.Conventions.IConventionScanner;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests
{
    public class ConventionTestWebAssemblyHostTests : AutoFakeTest
    {
        [Fact]
        public void Builder_Should_Create_Host()
        {
            Action a = () => TestWebAssemblyHost.For(this, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }


        [Fact]
        public void Builder_Should_Create_With_Delegate()
        {
            Action a = () => TestWebAssemblyHost.For(this, LoggerFactory)
               .Create(builder => { });
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Create_Host_ByType()
        {
            Action a = () => TestWebAssemblyHost.For(GetType(), LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Create_Host_ByAssembly()
        {
            Action a = () => TestWebAssemblyHost.For(GetType().Assembly, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Build_Host()
        {
            Action a = () => TestWebAssemblyHost.For(this, LoggerFactory)
               .Create()
               .Build();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IConventionScanner()
        {
            var builder = TestWebAssemblyHost.For(this, LoggerFactory)
               .Create(x => { x.UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>()); });

            builder.Scanner.Should().BeSameAs(AutoFake.Resolve<IConventionScanner>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyCandidateFinder()
        {
            var builder = TestWebAssemblyHost.For(this, LoggerFactory)
               .Create(x => { x.UseAssemblyCandidateFinder(AutoFake.Resolve<IAssemblyCandidateFinder>()); });

            builder.AssemblyCandidateFinder.Should().BeSameAs(AutoFake.Resolve<IAssemblyCandidateFinder>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyProvider()
        {
            var builder = TestWebAssemblyHost.For(this, LoggerFactory)
               .Create(
                    x => { x.UseAssemblyProvider(AutoFake.Resolve<IAssemblyProvider>()); }
                );

            builder.AssemblyProvider.Should().BeSameAs(AutoFake.Resolve<IAssemblyProvider>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_DiagnosticSource()
        {
            var builder = TestWebAssemblyHost.For(this, LoggerFactory)
               .WithLogger(Logger)
               .Create();

            builder.DiagnosticLogger.Should().BeSameAs(Logger);
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_ILogger()
        {
            Action a = () => TestWebAssemblyHost.For(this, LoggerFactory)
               .WithLogger(AutoFake.Resolve<ILogger>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Scan_For_Conventions_When_Desired()
        {
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .IncludeConventions()
               .Create();

            host.Scanner.BuildProvider().GetAll().Should().NotBeEmpty();
        }

        [Fact]
        public void Builder_Should_Not_Scan_For_Conventions()
        {
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .ExcludeConventions()
               .Create();

            host.Scanner.BuildProvider().GetAll().Should().BeEmpty();
        }

        [Fact]
        public void Builder_Should_Build()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(this, LoggerFactory).Create(
                x => { x.Services.AddSingleton<ServiceA>(); }
            );

            var host = testWebAssemblyHost.Build();
            host.Services.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate_Services()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(this, LoggerFactory).Create(
                x => { x.Services.AddSingleton<ServiceA>(); }
            );

            Populate(testWebAssemblyHost.Parse());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate_Container()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(this, LoggerFactory).Create(
                x => x.ConfigureContainer(new DryIocServiceProviderFactory(new Container()),
                    x => x.Register<ServiceA>(Reuse.Singleton))
            );

            Populate(testWebAssemblyHost.Parse<IContainer>());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Build_As_Many_Times_As_We_Want()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(this, LoggerFactory).Create(
                x =>
                {
                    x.ConfigureContainer(new DryIocServiceProviderFactory(new Container()),
                            x => x.Register<ServiceA>(Reuse.Singleton))
                        ;
                }
            );

            Populate(testWebAssemblyHost.Parse<IContainer>());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        class ServiceA
        {
            public string Value = nameof(ServiceA);
        }

        private static IConfiguration GetConfigurationFromChainedConfigurationProvider(IConfigurationProvider provider)
        {
            provider.Should().BeOfType<ChainedConfigurationProvider>();
            return ( typeof(ChainedConfigurationProvider).GetField(
                    "_config",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )!
               .GetValue(provider)! as IConfiguration )!;
        }

        [Fact]
        public void Builder_Should_Share_Configuration()
        {
            var configurationFake = A.Fake<IConfiguration>();
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .WithConfiguration(configurationFake)
               .Create();

            var rootConfiguration = host.Parse<IConfigurationRoot>();

            var configuration = GetConfigurationFromChainedConfigurationProvider(rootConfiguration.Providers.First());

            configuration.Should().Be(configurationFake);
        }

        [Theory]
        [InlineData(typeof(ConventionTestWebAssemblyHostTests))]
        [InlineData("stringkey")]
        [InlineData(1234)]
        public void Builder_Should_Reuse_Configuration(object key)
        {
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .ShareConfiguration(key)
               .Create();
            var host2 = TestWebAssemblyHost.For(this, LoggerFactory)
               .ShareConfiguration(key)
               .Create();

            var rootConfiguration = host.Parse<IConfigurationRoot>();
            var rootConfiguration2 = host2.Parse<IConfigurationRoot>();

            var configuration = GetConfigurationFromChainedConfigurationProvider(rootConfiguration.Providers.First());
            var configuration2 = GetConfigurationFromChainedConfigurationProvider(rootConfiguration2.Providers.First());

            configuration.Should().BeSameAs(configuration2);
        }

        [Fact]
        public void Builder_Should_Not_Reuse_Configuration_Across_Keys()
        {
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .ShareConfiguration(typeof(ConventionTestWebAssemblyHostTests))
               .Create();
            var host2 = TestWebAssemblyHost.For(this, LoggerFactory)
               .ShareConfiguration("stringkey")
               .Create();

            var rootConfiguration = host.Parse<IConfigurationRoot>();
            var rootConfiguration2 = host2.Parse<IConfigurationRoot>();

            var configuration = GetConfigurationFromChainedConfigurationProvider(rootConfiguration.Providers.First());
            var configuration2 = GetConfigurationFromChainedConfigurationProvider(rootConfiguration2.Providers.First());

            configuration.Should().NotBeSameAs(configuration2);
        }

        [Fact]
        public void Calls_Hosting_Conventions_When_Provided_Configuration()
        {
            var handler = A.Fake<WebAssemblyHostingConventionDelegate>();
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .WithConfiguration(AutoFake.Resolve<IConfiguration>())
               .Create()
               .ConfigureHosting(handler)
               .Get<IWebAssemblyHostBuilder>();

            var a = host.Build();
            A.CallTo(handler).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Calls_Hosting_Conventions_When_Sharing_Configuration()
        {
            var handler = A.Fake<WebAssemblyHostingConventionDelegate>();
            var host = TestWebAssemblyHost.For(this, LoggerFactory)
               .ShareConfiguration(typeof(ConventionTestWebAssemblyHostTests))
               .Create()
               .ConfigureHosting(handler)
               .Get<IWebAssemblyHostBuilder>();

            var a = host.Build();
            var b = host.Build();
            
            A.CallTo(handler).MustHaveHappenedTwiceExactly();
        }

        public ConventionTestWebAssemblyHostTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }
    }
}