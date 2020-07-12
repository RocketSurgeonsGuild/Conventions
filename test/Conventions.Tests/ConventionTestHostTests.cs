using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.Testing;
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
        public void Builder_Should_Not_Create_Host_When_Missing_AssemblyCandidateFinder()
        {
            Action a = () => new TestHost().With(A.Fake<IAssemblyProvider>()).Create();
            a.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "AssemblyCandidateFinder");
        }

        [Fact]
        public void Builder_Should_Not_Create_Host_When_Missing_AssemblyProvider()
        {
            Action a = () => new TestHost().With(A.Fake<IAssemblyCandidateFinder>()).Create();
            a.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "AssemblyProvider");
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
        public void Builder_Should_Parse_Host()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .Create()
               .Parse();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IConventionScanner()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IConventionScanner>())
               .Create();

            builder.Scanner.Should().BeSameAs(AutoFake.Resolve<IConventionScanner>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyCandidateFinder()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IAssemblyCandidateFinder>())
               .Create();

            builder.AssemblyCandidateFinder.Should().BeSameAs(AutoFake.Resolve<IAssemblyCandidateFinder>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyProvider()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IAssemblyProvider>())
               .Create();

            builder.AssemblyProvider.Should().BeSameAs(AutoFake.Resolve<IAssemblyProvider>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IServiceProviderDictionary()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IServiceProviderDictionary>())
               .Create();

            builder.ServiceProperties.Should().BeSameAs(AutoFake.Resolve<IServiceProviderDictionary>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_DiagnosticSource()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<DiagnosticSource>())
               .Create();

            builder.DiagnosticSource.Should().BeSameAs(AutoFake.Resolve<DiagnosticSource>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_ILogger()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<ILogger>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IHostEnvironment()
        {
            Action a = () => TestHost.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IHostEnvironment>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Scan_For_Conventions_When_Desired()
        {
            var host = TestHost.For(this, LoggerFactory)
               .Create()
               .IncludeConventionAttributes();

            host.Scanner.BuildProvider().GetAll().Should().NotBeEmpty();
        }

        [Fact]
        public void Builder_Should_Not_Scan_For_Conventions()
        {
            var host = TestHost.For(this, LoggerFactory)
               .Create()
               .ExcludeConventionAttributes();

            host.Scanner.BuildProvider().GetAll().Should().BeEmpty();
        }

        [Fact]
        public void Builder_Should_Add_Configuration()
        {
            var host = TestHost.For(this, LoggerFactory)
               .WithConfigOptions(new ConfigOptions().UseJson().UseYaml().UseYml().UseIni())
               .Create();

            var (rootConfiguration, _) = host.Build();

            var provider = rootConfiguration.Providers.First();
            provider.Should().BeOfType<ChainedConfigurationProvider>();
            var configuration = typeof(ChainedConfigurationProvider).GetField(
                    "_config",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )!
               .GetValue(provider) as IConfigurationRoot;

            configuration!.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
            configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
            configuration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(3);
        }

        [Fact]
        public void Builder_Should_Add_Configuration_After_It_Has_Been_Changed()
        {
            var host = TestHost.For(this, LoggerFactory)
               .Create();
            host.GetOrAdd(() => new ConfigOptions());

            var (rootConfiguration, _) = host.Build();

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
            return (typeof(ChainedConfigurationProvider).GetField(
                    "_config",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )!
               .GetValue(provider)! as IConfiguration)!;
        }

        [Fact]
        public void Builder_Should_Share_Configuration()
        {
            var configurationFake = A.Fake<IConfiguration>();
            var host = TestHost.For(this, LoggerFactory)
               .WithConfiguration(configurationFake)
               .Create();

            var (rootConfiguration, _) = host.Build();

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

            var (rootConfiguration, _) = host.Build();
            var (rootConfiguration2, _) = host2.Build();

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

            var (rootConfiguration, _) = host.Build();
            var (rootConfiguration2, _) = host2.Build();

            var configuration = GetConfigurationFromChainedConfigurationProvider(rootConfiguration.Providers.First());
            var configuration2 = GetConfigurationFromChainedConfigurationProvider(rootConfiguration2.Providers.First());

            configuration.Should().NotBeSameAs(configuration2);
        }

        public ConventionTestHostTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }
    }
}