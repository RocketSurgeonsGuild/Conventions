﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection.Tests;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

[assembly: Convention(typeof(ServiceBuilderTests.AbcConvention))]

namespace Rocket.Surgery.Extensions.DependencyInjection.Tests
{
    public class ServiceBuilderTests : AutoTestBase
    {
        public ServiceBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
        }

        [Fact]
        public void Constructs()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();

            Action a = () => { servicesBuilder.PrependConvention(A.Fake<IServiceConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();

            var convention = A.Fake<IServiceConvention>();

            servicesBuilder.PrependConvention(convention);

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IEnumerable<IServiceConvention>>._)).MustHaveHappened();
        }

        public interface IAbc { }
        public interface IAbc2 { }
        public interface IAbc3 { }
        public interface IAbc4 { }

        public class AbcConvention : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                context.Services.AddSingleton(A.Fake<IAbc>());
                context.Services.AddSingleton(A.Fake<IAbc2>());
            }
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection()); ;
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc>());
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());

            var sp = servicesBuilder.Build();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc>());
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc4>());

            var sp = servicesBuilder.Build();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc3>());
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc4>());

            var sp = servicesBuilder.Build();
            sp.GetService<IAbc>().Should().BeNull();
            sp.GetService<IAbc2>().Should().BeNull();
            sp.GetService<IAbc3>().Should().NotBeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build();
            items.GetService<IAbc>().Should().NotBeNull();
            items.GetService<IAbc2>().Should().NotBeNull();
            items.GetService<IAbc3>().Should().BeNull();
            items.GetService<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ServicesBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerApplication = A.Fake<IObserver<IServiceProvider>>();
            var observerSystem = A.Fake<IObserver<IServiceProvider>>();
            servicesBuilder.OnBuild.Subscribe(observer);

            var serviceProvider = servicesBuilder.Build();

            A.CallTo(() => observer.OnNext(serviceProvider)).MustHaveHappenedOnceExactly();
        }
    }
}
