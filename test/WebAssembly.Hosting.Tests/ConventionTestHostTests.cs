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
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.WebAssembly.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests
{
    public class ConventionTestWebAssemblyHostTests : AutoFakeTest
    {
        [Fact]
        public void Builder_Should_Create_Host_ByAssemblies()
        {
            Action a = () => TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Build_Host_ByAppDomain()
        {
            Action a = () => TestWebAssemblyHost.For(AppDomain.CurrentDomain, LoggerFactory)
               .Create()
               .Build();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_DiagnosticSource()
        {
            var builder = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .WithLogger(Logger)
               .Create();

            builder.Logger.Should().BeSameAs(Logger);
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_ILogger()
        {
            Action a = () => TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .WithLogger(AutoFake.Resolve<ILogger>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Scan_For_Conventions_When_Desired()
        {
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .IncludeConventions()
               .Create();

            host.Conventions.GetAll().Should().NotBeEmpty();
        }

        [Fact]
        public void Builder_Should_Not_Scan_For_Conventions()
        {
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .ExcludeConventions()
               .Create();

            host.Conventions.GetAll().Should().BeEmpty();
        }

        [Fact]
        public void Builder_Should_Build()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory).Create(
                x => { x.ConfigureServices((context, configuration, services) => services.AddSingleton<ServiceA>()); }
            );

            var host = testWebAssemblyHost.Build();
            host.Services.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate_Services()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory).Create(
                x => { x.ConfigureServices((context, configuration, services) => services.AddSingleton<ServiceA>()); }
            );

            Populate(testWebAssemblyHost.Parse());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate_Container()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(new[] { GetType().Assembly }, LoggerFactory).Create().Configure(
                x => x.ConfigureContainer(new DryIocServiceProviderFactory(new Container()), x => x.Register<ServiceA>(Reuse.Singleton))
            );

            Populate(testWebAssemblyHost.Parse<IContainer>());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Build_As_Many_Times_As_We_Want()
        {
            var testWebAssemblyHost = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory).Create().Configure(
                x => x.ConfigureContainer(new DryIocServiceProviderFactory(new Container()), x => x.Register<ServiceA>(Reuse.Singleton))
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
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
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
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .ShareConfiguration(key)
               .Create();
            var host2 = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
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
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .ShareConfiguration(typeof(ConventionTestWebAssemblyHostTests))
               .Create();
            var host2 = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
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
            var handler = A.Fake<ServiceConvention>();
            var handler2 = A.Fake<LoggingConvention>();
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .WithConfiguration(AutoFake.Resolve<IConfiguration>())
               .Create(x => x
                   .ConfigureServices(handler)
               .ConfigureLogging(handler2)
                   .ConfigureLogging(handler2)
                );

            var a = host.Build();
            A.CallTo(handler).MustHaveHappenedOnceExactly();
            A.CallTo(handler2).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Calls_Hosting_Conventions_When_Sharing_Configuration()
        {
            var handler = A.Fake<ServiceConvention>();
            var handler2 = A.Fake<LoggingConvention>();
            var host = TestWebAssemblyHost.For(new [] { GetType().Assembly }, LoggerFactory)
               .ShareConfiguration(typeof(ConventionTestWebAssemblyHostTests))
               .Create(x => x
                   .ConfigureServices(handler)
               .ConfigureLogging(handler2)
                   .ConfigureLogging(handler2)
                );

            var a = host.Build();
            var b = host.Build();

            A.CallTo(handler).MustHaveHappenedTwiceExactly();
            A.CallTo(handler2).MustHaveHappenedTwiceExactly();
        }

        public ConventionTestWebAssemblyHostTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }
    }
}