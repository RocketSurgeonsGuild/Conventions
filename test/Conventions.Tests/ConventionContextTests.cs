using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Setup;
using Rocket.Surgery.Extensions.Testing;
using Serilog.Events;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionContextTests
    (ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
{
    [Fact]
    public async Task ReturnsNullOfNoValue()
    {
        var builder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var context = await ConventionContext.FromAsync(builder);
        context[typeof(string)].Should().BeNull();
    }

    [Fact]
    public async Task SetAValue()
    {
        var builder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var container = await ConventionContext.FromAsync(builder);
        container[typeof(string)] = "abc";
        container[typeof(string)].Should().Be("abc");
    }

    [Fact]
    public async Task StoresAndReturnsItems()
    {
        var builder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var context = await ConventionContext.FromAsync(builder);

        var value = new object();
        context[string.Empty] = value;
        context[string.Empty].Should().BeSameAs(value);
    }

    [Fact]
    public async Task IgnoreNonExistentItems()
    {
        var builder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var context = await ConventionContext.FromAsync(builder);

        context[string.Empty].Should().BeNull();
    }

    [Fact]
    public async Task GetAStronglyTypedValue()
    {
        var builder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var container = await ConventionContext.FromAsync(builder);
        container[typeof(string)] = "abc";
        container.Get<string>().Should().Be("abc");
    }

    [Fact]
    public async Task SetAStronglyTypedValue()
    {
        var builder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var container = await ConventionContext.FromAsync(builder);
        container.Set("abc");
        container.Get<string>().Should().Be("abc");
    }

    [Fact]
    public Task AddConventions()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary(), []);
        var convention = A.Fake<IServiceConvention>();
        contextBuilder.PrependConvention(convention);

        ConventionContextHelpers.CreateProvider(contextBuilder, new TestAssemblyProvider(), Logger).GetAll().Should().Contain(convention);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Setups()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary(), [])
           .UseAssemblies(Array.Empty<Assembly>());
        var convention = A.Fake<ISetupConvention>();
        contextBuilder.PrependConvention(convention);

        var context = await ConventionContext.FromAsync(contextBuilder);
        A.CallTo(() => convention.Register(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Setups_With_Delegate()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary(), [])
           .UseAssemblies(Array.Empty<Assembly>());
        var convention = A.Fake<SetupConvention>();
        contextBuilder.SetupConvention(convention);

        var context = await ConventionContext.FromAsync(contextBuilder);
        A.CallTo(() => convention(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary(), [])
                            .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                            .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(contextBuilder);

        var servicesCollection = await new ServiceCollection()
                                      .AddSingleton(A.Fake<IAbc>())
                                      .AddSingleton(A.Fake<IAbc2>())
                                      .ApplyConventionsAsync(context);

        var sp = servicesCollection.BuildServiceProvider();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary(), [])
                            .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                            .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(contextBuilder);

        var servicesCollection = await new ServiceCollection().ApplyConventionsAsync(context);

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
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var contextBuilder = new ConventionContextBuilder(new ServiceProviderDictionary(), [])
                            .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                            .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(contextBuilder);

        var servicesCollection = await new ServiceCollection().ApplyConventionsAsync(context);
        servicesCollection.AddSingleton(A.Fake<IAbc3>());
        servicesCollection.AddSingleton(A.Fake<IAbc4>());

        var sp = servicesCollection.BuildServiceProvider();
        sp.GetService<IAbc>().Should().BeNull();
        sp.GetService<IAbc2>().Should().BeNull();
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = AutoFake
                     .Resolve<ConventionContextBuilder>()
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .AppendConvention(new AbcConvention());
        builder.Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(builder);
        var servicesCollection = await new ServiceCollection().ApplyConventionsAsync(context);

        var items = servicesCollection.BuildServiceProvider();
        items.GetService<IAbc>().Should().NotBeNull();
        items.GetService<IAbc2>().Should().NotBeNull();
        items.GetService<IAbc3>().Should().BeNull();
        items.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ShouldConstructTheConventionInjectingTheValues()
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        var data = A.Fake<IInjectData>();
        var builder = AutoFake
                     .Resolve<ConventionContextBuilder>()
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .AppendConvention<InjectableConvention>()
                     .Set(data)
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(builder);
        var collection = await new ServiceCollection().ApplyConventionsAsync(context);
        collection.Should().Contain(z => z.ServiceType == typeof(IInjectData));
    }

    [Fact]
    public async Task ShouldConstructTheConventionInjectingTheValuesIfOptional()
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        var data = A.Fake<IInjectData>();
        var builder = AutoFake
                     .Resolve<ConventionContextBuilder>()
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .AppendConvention<OptionalInjectableConvention>()
                     .Set(data)
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = ( await ConventionContext.FromAsync(builder) ).Set(data);
        var collection = await new ServiceCollection().ApplyConventionsAsync(context);
        collection.Should().Contain(z => z.ServiceType == typeof(IInjectData));
    }

    [Fact]
    public async Task ShouldFailToConstructTheConventionInjectingTheValuesIfMissing()
    {
        var builder = AutoFake
                     .Resolve<ConventionContextBuilder>()
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .AppendConvention<InjectableConvention>()
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var a = () => ConventionContext.FromAsync(builder).AsTask();
        await a.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldNotFailToConstructTheConventionInjectingTheValuesIfOptional()
    {
        var builder = AutoFake
                     .Resolve<ConventionContextBuilder>()
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .AppendConvention<OptionalInjectableConvention>()
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(builder);
        var a = () => new ServiceCollection().ApplyConventionsAsync(context).AsTask();
        await a.Should().NotThrowAsync<InvalidOperationException>();
    }

    [field: AllowNull]
    [field: MaybeNull]
    private ILoggerFactory LoggerFactory => field ??= CreateLoggerFactory();

    private ILogger Logger => LoggerFactory.CreateLogger(GetType());

    public interface IAbc;

    public interface IAbc2;

    public interface IAbc3;

    public interface IAbc4;

    public interface IInjectData;

    public class InjectData;

    public class AbcConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(context);

            services.AddSingleton(A.Fake<IAbc>());
            services.AddSingleton(A.Fake<IAbc2>());
        }
    }

    public class InjectableConvention(IInjectData convention) : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(context);

            services.AddSingleton(convention);
        }
    }

    public class OptionalInjectableConvention(IInjectData? convention = null) : IServiceAsyncConvention
    {
        public ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (convention is { })
                services.AddSingleton(convention);
            return ValueTask.CompletedTask;
        }
    }
}
