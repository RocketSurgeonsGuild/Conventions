using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTestHostTests : AutoFakeTest
    {
        [Fact]
        public void Builder_Should_Create_Host()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }


        [Fact]
        public void Builder_Should_Create_With_Delegate()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .Create(builder => { });
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Create_Host_ByType()
        {
            Action a = () => TestHost.For(GetType(), LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Create_Host_ByAssembly()
        {
            Action a = () => TestHost.For(GetType().Assembly, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Build_Host()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .Create()
               .Build();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_ILogger()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .WithLogger(AutoFake.Resolve<ILogger>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Build()
        {
            var testHost = TestHost.For(this, LoggerFactory).Create(
                x => x.ConfigureServices((c, _, x) => x.AddSingleton<ServiceA>())
            );

            using var host = testHost.Build();
            host.Services.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate_Services()
        {
            var testHost = TestHost.For(this, LoggerFactory).Create(
                x => { x.ConfigureServices((c, _, x) => x.AddSingleton<ServiceA>()); }
            );

            Populate(testHost.Parse());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate_Container()
        {
            var testHost = TestHost.For(this, LoggerFactory).Create().Configure(
                x =>
                    x.UseServiceProviderFactory(new DryIocServiceProviderFactory(new Container()))
                       .ConfigureContainer<IContainer>(x => x.Register<ServiceA>(Reuse.Singleton))
            );

            Populate(testHost.Parse<IContainer>());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Build_As_Many_Times_As_We_Want()
        {
            var testHost = TestHost.For(this, LoggerFactory).Create().Configure(
                z => z.UseServiceProviderFactory(new DryIocServiceProviderFactory(new Container()))
                   .ConfigureContainer<IContainer>(x => x.Register<ServiceA>(Reuse.Singleton))
            );

            Populate(testHost.Parse<IContainer>());
            Container.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        class ServiceA
        {
            public string Value = nameof(ServiceA);
        }

        [Fact]
        public void Builder_Should_Add_Configuration()
        {
            var host = TestHost.For(this, LoggerFactory)
               .Create()
               .Set(new ConfigOptions().UseJson().UseYaml().UseYml().UseIni());

            var rootConfiguration = host.Parse<IConfigurationRoot>();

            rootConfiguration!.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
            rootConfiguration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
            rootConfiguration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(3);
        }

        [Fact]
        public void Builder_Should_Add_Configuration_After_It_Has_Been_Changed()
        {
            var host = TestHost.For(this, LoggerFactory).Create();
            host.Properties.GetOrAdd(() => new ConfigOptions());

            var rootConfiguration = host.Parse<IConfigurationRoot>();

            var provider = rootConfiguration.Providers.First();
            provider.Should().BeOfType<ChainedConfigurationProvider>();
            var configuration = typeof(ChainedConfigurationProvider).GetField(
                    "_config",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )!
               .GetValue(provider) as IConfigurationRoot;

            configuration!.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(0);
            configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(0);
            configuration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(0);
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
            var host = TestHost.For(this, LoggerFactory)
               .WithConfiguration(configurationFake)
               .Create();

            var rootConfiguration = host.Parse<IConfigurationRoot>();

            var configuration = GetConfigurationFromChainedConfigurationProvider(rootConfiguration.Providers.First());

            configuration.Should().Be(configurationFake);
        }

        [Theory]
        [InlineData(typeof(ConventionTestHostTests))]
        [InlineData("stringkey")]
        [InlineData(1234)]
        public void Builder_Should_Reuse_Configuration(object key)
        {
            var host = TestHost.For(this, LoggerFactory)
               .ShareConfiguration(key)
               .Create();
            var host2 = TestHost.For(this, LoggerFactory)
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
            var host = TestHost.For(this, LoggerFactory)
               .ShareConfiguration(typeof(ConventionTestHostTests))
               .Create();
            var host2 = TestHost.For(this, LoggerFactory)
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
            var handler = A.Fake<HostingConvention>();
            var host = TestHost.For(this, LoggerFactory)
               .WithConfiguration(AutoFake.Resolve<IConfiguration>())
               .Create(x => x.ConfigureHosting(handler))
               .Get<IHostBuilder>();

            using (var a = host.Build()) { }

            A.CallTo(handler).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Calls_Hosting_Conventions_When_Sharing_Configuration()
        {
            var handler = A.Fake<HostingConvention>();
            var host = TestHost.For(this, LoggerFactory)
               .ShareConfiguration(typeof(ConventionTestHostTests))
               .Create(x => x.ConfigureHosting(handler))
               .Get<IHostBuilder>();

            using (var a = host.Build()) { }

            using (var a = host.Build()) { }

            A.CallTo(handler).MustHaveHappenedTwiceExactly();
        }

        public ConventionTestHostTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }
    }
}