using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DryIoc;
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
using Rocket.Surgery.Conventions.Tests.DependencyInjection;
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
        public void Builder_Should_Use_A_Custom_IConventionScanner()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .Create(x => { x.UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>()); });

            builder.Scanner.Should().BeSameAs(AutoFake.Resolve<IConventionScanner>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyCandidateFinder()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .Create(x => { x.UseAssemblyCandidateFinder(AutoFake.Resolve<IAssemblyCandidateFinder>()); });

            builder.AssemblyCandidateFinder.Should().BeSameAs(AutoFake.Resolve<IAssemblyCandidateFinder>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyProvider()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .Create(
                    x => { x.UseAssemblyProvider(AutoFake.Resolve<IAssemblyProvider>()); }
                );

            builder.AssemblyProvider.Should().BeSameAs(AutoFake.Resolve<IAssemblyProvider>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_DiagnosticSource()
        {
            var builder = TestHost.For(this, LoggerFactory)
               .With(Logger)
               .Create();

            builder.DiagnosticLogger.Should().BeSameAs(Logger);
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
        public void Builder_Should_Scan_For_Conventions_When_Desired()
        {
            var host = TestHost.For(this, LoggerFactory)
               .IncludeConventionAttributes()
               .Create();

            host.Scanner.BuildProvider().GetAll().Should().NotBeEmpty();
        }

        [Fact]
        public void Builder_Should_Not_Scan_For_Conventions()
        {
            var host = TestHost.For(this, LoggerFactory)
               .ExcludeConventionAttributes()
               .Create();

            host.Scanner.BuildProvider().GetAll().Should().BeEmpty();
        }

        [Fact]
        public void Builder_Should_Build()
        {
            var testHost = TestHost.For(this, LoggerFactory).Create(
                x => { x.ConfigureServices(x => x.AddSingleton<ServiceA>()); }
            );

            using var host = testHost.Build();
            host.Services.GetRequiredService<ServiceA>().Should().NotBeNull();
        }

        [Fact]
        public void Builder_Should_Populate()
        {
            var services = TestHost.For(this, LoggerFactory).Create(
                x => { x.ConfigureServices(x => x.AddSingleton<ServiceA>()); }
            ).Parse();

            Populate(services);
            Container.IsRegistered<ServiceA>().Should().BeTrue();
        }

        [Fact]
        public void Builder_Should_Populate_DryIoc()
        {
            var testHost = TestHost.For(this, LoggerFactory).Create(
                x =>
                {
                    var container = new Container();
                    container.UseInstance(new ServiceA());

                    x.UseServiceProviderFactory(context =>
                        new ServicesBuilderServiceProviderFactory(
                            collection =>
                                new DryIocBuilder(
                                    context.HostingEnvironment,
                                    context.Configuration,
                                    x.Scanner,
                                    x.AssemblyProvider,
                                    x.AssemblyCandidateFinder,
                                    collection,
                                    container,
                                    x.Get<ILogger>(),
                                    x.ServiceProperties
                                )
                        )
                    );
                }
            );
                testHost.Parse();

            Populate(testHost.Get<IContainer>());
            Container.IsRegistered<ServiceA>().Should().BeTrue();
        }

        class ServiceA
        {
            public string Value = nameof(ServiceA);
        }

        public ConventionTestHostTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }
    }
}