using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Tests;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Conventions.Tests.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionContextTests : AutoFakeTest
    {
        [Fact]
        public void Constructs()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>() { [typeof(IHostEnvironment)] = AutoFake.Resolve<IHostEnvironment>() });
            var servicesBuilder = AutoFake.Resolve<ConventionContext>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Properties.Should().ContainKey(typeof(IHostEnvironment));
        }

        [Fact]
        public void ReturnsNullOfNoValue()
        {
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>());
            var container = AutoFake.Resolve<ConventionContext>();
            container[typeof(string)].Should().BeNull();
        }


        [Fact]
        public void SetAValue()
        {
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>());
            var container = AutoFake.Resolve<ConventionContext>();
            container[typeof(string)] = "abc";
            container[typeof(string)].Should().Be("abc");
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>());
            var servicesBuilder = AutoFake.Resolve<ConventionContext>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<ConventionContext>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void GetAStronglyTypedValue()
        {
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>());
            var container = AutoFake.Resolve<ConventionContext>();
            container[typeof(string)] = "abc";
            container.Get<string>().Should().Be("abc");
        }

        [Fact]
        public void SetAStronglyTypedValue()
        {
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>());
            var container = AutoFake.Resolve<ConventionContext>();
            container.Set("abc");
            container.Get<string>().Should().Be("abc");
        }

        [Fact]
        public void AddConventions()
        {
            var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>());
            var convention = A.Fake<IServiceConvention>();
            contextBuilder.PrependConvention(convention);

            ConventionContextHelpers.CreateProvider(contextBuilder, new TestAssemblyProvider(), Logger).GetAll().Should().Contain(convention);
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>())
               .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
               .Set<IConfiguration>(new ConfigurationBuilder().Build());
            var context = ConventionContext.From(contextBuilder);

            var servicesCollection = new ServiceCollection()
               .AddSingleton(A.Fake<IAbc>())
               .AddSingleton(A.Fake<IAbc2>())
               .ApplyConventions(context);

            var sp = servicesCollection.BuildServiceProvider();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>())
               .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
               .Set<IConfiguration>(new ConfigurationBuilder().Build());
            var context = ConventionContext.From(contextBuilder);

            var servicesCollection = new ServiceCollection().ApplyConventions(context);

            servicesCollection.AddSingleton(A.Fake<IAbc>());
            servicesCollection.AddSingleton(A.Fake<IAbc2>());
            servicesCollection.AddSingleton(A.Fake<IAbc4>());

            var sp = servicesCollection.BuildServiceProvider();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            var contextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>())
               .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
               .Set<IConfiguration>(new ConfigurationBuilder().Build());
            var context = ConventionContext.From(contextBuilder);

            var servicesCollection = new ServiceCollection().ApplyConventions(context);
            servicesCollection.AddSingleton(A.Fake<IAbc3>());
            servicesCollection.AddSingleton(A.Fake<IAbc4>());

            var sp = servicesCollection.BuildServiceProvider();
            sp.GetService<IAbc>().Should().BeNull();
            sp.GetService<IAbc2>().Should().BeNull();
            sp.GetService<IAbc3>().Should().NotBeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            AutoFake.Provide<IDictionary<object, object?>>(new Dictionary<object, object?>());
            var builder = AutoFake.Resolve<ConventionContextBuilder>().UseAssemblies(new TestAssemblyProvider().GetAssemblies()).AppendConvention(new AbcConvention());
            builder.Set<IConfiguration>(new ConfigurationBuilder().Build());
            var context = ConventionContext.From(builder);
            var servicesCollection = new ServiceCollection().ApplyConventions(context);

            var items = servicesCollection.BuildServiceProvider();
            items.GetService<IAbc>().Should().NotBeNull();
            items.GetService<IAbc2>().Should().NotBeNull();
            items.GetService<IAbc3>().Should().BeNull();
            items.GetService<IAbc4>().Should().BeNull();
        }

        public ConventionContextTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        public interface IAbc { }

        public interface IAbc2 { }

        public interface IAbc3 { }

        public interface IAbc4 { }

        public class AbcConvention : IServiceConvention
        {
            public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                services.AddSingleton(A.Fake<IAbc>());
                services.AddSingleton(A.Fake<IAbc2>());
            }
        }
    }
}