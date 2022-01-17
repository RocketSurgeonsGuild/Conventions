using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Setup;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionContextTests : AutoFakeTest
{
    [Fact]
    public void Constructs()
    {
        var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary { [typeof(IConvention)] = new AbcConvention() });
        var servicesBuilder = AutoFake.Resolve<ConventionContext>();

        servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
        servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
        servicesBuilder.Properties.Should().ContainKey(typeof(IConvention));
    }

    [Fact]
    public void ReturnsNullOfNoValue()
    {
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var container = AutoFake.Resolve<ConventionContext>();
        container[typeof(string)].Should().BeNull();
    }


    [Fact]
    public void SetAValue()
    {
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var container = AutoFake.Resolve<ConventionContext>();
        container[typeof(string)] = "abc";
        container[typeof(string)].Should().Be("abc");
    }

    [Fact]
    public void StoresAndReturnsItems()
    {
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
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
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var container = AutoFake.Resolve<ConventionContext>();
        container[typeof(string)] = "abc";
        container.Get<string>().Should().Be("abc");
    }

    [Fact]
    public void SetAStronglyTypedValue()
    {
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var container = AutoFake.Resolve<ConventionContext>();
        container.Set("abc");
        container.Get<string>().Should().Be("abc");
    }

    [Fact]
    public void AddConventions()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary());
        var convention = A.Fake<IServiceConvention>();
        contextBuilder.PrependConvention(convention);

        ConventionContextHelpers.CreateProvider(contextBuilder, new TestAssemblyProvider(), Logger).GetAll().Should().Contain(convention);
    }

    [Fact]
    public void Setups()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary())
           .UseAssemblies(Array.Empty<Assembly>());
        var convention = A.Fake<ISetupConvention>();
        contextBuilder.PrependConvention(convention);

        var context = ConventionContext.From(contextBuilder);
        A.CallTo(() => convention.Register(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Setups_With_Delegate()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary())
           .UseAssemblies(Array.Empty<Assembly>());
        var convention = A.Fake<SetupConvention>();
        contextBuilder.SetupConvention(convention);

        var context = ConventionContext.From(contextBuilder);
        A.CallTo(() => convention(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary())
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
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary())
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
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary())
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
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var builder = AutoFake.Resolve<ConventionContextBuilder>().UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                              .AppendConvention(new AbcConvention());
        builder.Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = ConventionContext.From(builder);
        var servicesCollection = new ServiceCollection().ApplyConventions(context);

        var items = servicesCollection.BuildServiceProvider();
        items.GetService<IAbc>().Should().NotBeNull();
        items.GetService<IAbc2>().Should().NotBeNull();
        items.GetService<IAbc3>().Should().BeNull();
        items.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public void ShouldConstructTheConventionInjectingTheValues()
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        var data = A.Fake<IInjectData>();
        var builder = AutoFake.Resolve<ConventionContextBuilder>()
                              .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                              .AppendConvention<InjectableConvention>()
                              .Set(data)
                              .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = ConventionContext.From(builder);
        var collection = new ServiceCollection().ApplyConventions(context);
        collection.Should().Contain(z => z.ServiceType == typeof(IInjectData));
    }

    [Fact]
    public void ShouldConstructTheConventionInjectingTheValuesIfOptional()
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        var data = A.Fake<IInjectData>();
        var builder = AutoFake.Resolve<ConventionContextBuilder>()
                              .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                              .AppendConvention<OptionalInjectableConvention>()
                              .Set(data)
                              .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = ConventionContext.From(builder).Set(data);
        var collection = new ServiceCollection().ApplyConventions(context);
        collection.Should().Contain(z => z.ServiceType == typeof(IInjectData));
    }

    [Fact]
    public void ShouldFailToConstructTheConventionInjectingTheValuesIfMissing()
    {
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var builder = AutoFake.Resolve<ConventionContextBuilder>()
                              .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                              .AppendConvention<InjectableConvention>()
                              .Set<IConfiguration>(new ConfigurationBuilder().Build());
        Action a = () => ConventionContext.From(builder);
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ShouldNotFailToConstructTheConventionInjectingTheValuesIfOptional()
    {
        AutoFake.Provide<IServiceProviderDictionary>(new ServiceProviderDictionary());
        var builder = AutoFake.Resolve<ConventionContextBuilder>()
                              .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                              .AppendConvention<OptionalInjectableConvention>()
                              .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = ConventionContext.From(builder);
        Action a = () => new ServiceCollection().ApplyConventions(context);
        a.Should().NotThrow<InvalidOperationException>();
    }

    public ConventionContextTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    public interface IAbc
    {
    }

    public interface IAbc2
    {
    }

    public interface IAbc3
    {
    }

    public interface IAbc4
    {
    }

    public interface IInjectData
    {
    }

    public class InjectData
    {
    }

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

    public class InjectableConvention : IServiceConvention
    {
        private readonly IInjectData _convention;

        public InjectableConvention(IInjectData convention)
        {
            _convention = convention;
        }

        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.AddSingleton(_convention);
        }
    }

    public class OptionalInjectableConvention : IServiceConvention
    {
        private readonly IInjectData? _convention;

        public OptionalInjectableConvention(IInjectData? convention = null)
        {
            _convention = convention;
        }

        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (_convention is { })
                services.AddSingleton(_convention);
        }
    }
}
