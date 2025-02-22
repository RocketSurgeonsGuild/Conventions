using FakeItEasy;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Setup;
using Rocket.Surgery.Extensions.Testing;

using Serilog.Events;

using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionContextTests
    (ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
{
    [Fact]
    public async Task GetAStronglyTypedValue()
    {
        var builder = ConventionContextBuilder.Create(b => [], new ServiceProviderDictionary());
        var container = await ConventionContext.FromAsync(builder);
        container.Set("abc");
        container.Get<string>().ShouldBe("abc");
    }

    [Fact]
    public async Task SetAStronglyTypedValue()
    {
        var builder = ConventionContextBuilder.Create(b => [], new ServiceProviderDictionary());
        var container = await ConventionContext.FromAsync(builder);
        container.Set("abc");
        container.Get<string>().ShouldBe("abc");
    }

    [Fact]
    public async Task AddConventions()
    {
        var contextBuilder = ConventionContextBuilder.Create(b => [], new ServiceProviderDictionary());
        var convention = A.Fake<IServiceConvention>();
        contextBuilder.PrependConvention(convention);
        var conventions = await ConventionContext.FromAsync(contextBuilder);
        conventions.Conventions.GetAll().ShouldContain(convention);
    }

    [Fact]
    public async Task Setups()
    {
        var contextBuilder = ConventionContextBuilder.Create(b => [], new ServiceProviderDictionary());
        var convention = A.Fake<ISetupConvention>();
        contextBuilder.PrependConvention(convention);

        var context = await ConventionContext.FromAsync(contextBuilder);
        A.CallTo(() => convention.Register(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Setups_With_Delegate()
    {
        var contextBuilder = ConventionContextBuilder.Create(b => [], new ServiceProviderDictionary());
        var convention = A.Fake<SetupConvention>();
        contextBuilder.SetupConvention(convention);

        var context = await ConventionContext.FromAsync(contextBuilder);
        A.CallTo(() => convention(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var contextBuilder = ConventionContextBuilder
                            .Create(b => [], new ServiceProviderDictionary())
                            .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(contextBuilder);

        var servicesCollection = await new ServiceCollection()
                                      .AddSingleton(A.Fake<IAbc>())
                                      .AddSingleton(A.Fake<IAbc2>())
                                      .ApplyConventionsAsync(context);

        var sp = servicesCollection.BuildServiceProvider();
        sp.GetService<IAbc>().ShouldNotBeNull();
        sp.GetService<IAbc2>().ShouldNotBeNull();
        sp.GetService<IAbc3>().ShouldBeNull();
        sp.GetService<IAbc4>().ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var contextBuilder = ConventionContextBuilder
                            .Create(b => [], new ServiceProviderDictionary())
                            .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(contextBuilder);

        var servicesCollection = await new ServiceCollection().ApplyConventionsAsync(context);

        servicesCollection.AddSingleton(A.Fake<IAbc>());
        servicesCollection.AddSingleton(A.Fake<IAbc2>());
        servicesCollection.AddSingleton(A.Fake<IAbc4>());

        var sp = servicesCollection.BuildServiceProvider();
        sp.GetService<IAbc>().ShouldNotBeNull();
        sp.GetService<IAbc2>().ShouldNotBeNull();
        sp.GetService<IAbc3>().ShouldBeNull();
        sp.GetService<IAbc4>().ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var contextBuilder = ConventionContextBuilder
                            .Create(b => [], new ServiceProviderDictionary())
                            .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(contextBuilder);

        var servicesCollection = await new ServiceCollection().ApplyConventionsAsync(context);
        servicesCollection.AddSingleton(A.Fake<IAbc3>());
        servicesCollection.AddSingleton(A.Fake<IAbc4>());

        var sp = servicesCollection.BuildServiceProvider();
        sp.GetService<IAbc>().ShouldBeNull();
        sp.GetService<IAbc2>().ShouldBeNull();
        sp.GetService<IAbc3>().ShouldNotBeNull();
        sp.GetService<IAbc4>().ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = ConventionContextBuilder
                     .Create(_ => [])
                     .AppendConvention(new AbcConvention());
        builder.Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(builder);
        var servicesCollection = await new ServiceCollection().ApplyConventionsAsync(context);

        var items = servicesCollection.BuildServiceProvider();
        items.GetService<IAbc>().ShouldNotBeNull();
        items.GetService<IAbc2>().ShouldNotBeNull();
        items.GetService<IAbc3>().ShouldBeNull();
        items.GetService<IAbc4>().ShouldBeNull();
    }

    [Fact]
    public async Task ShouldConstructTheConventionInjectingTheValues()
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        var data = A.Fake<IInjectData>();
        var builder = ConventionContextBuilder
                     .Create(_ => [])
                     .AppendConvention<InjectableConvention>()
                     .Set(data)
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(builder);
        var collection = await new ServiceCollection().ApplyConventionsAsync(context);
        collection.ShouldContain(z => z.ServiceType == typeof(IInjectData));
    }

    [Fact]
    public async Task ShouldConstructTheConventionInjectingTheValuesIfOptional()
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
        var data = A.Fake<IInjectData>();
        var builder = ConventionContextBuilder
                     .Create(_ => [])
                     .AppendConvention<OptionalInjectableConvention>()
                     .Set(data)
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = ( await ConventionContext.FromAsync(builder) ).Set(data);
        var collection = await new ServiceCollection().ApplyConventionsAsync(context);
        collection.ShouldContain(z => z.ServiceType == typeof(IInjectData));
    }

    [Fact]
    public async Task ShouldFailToConstructTheConventionInjectingTheValuesIfMissing()
    {
        var builder = ConventionContextBuilder
                     .Create(_ => [])
                     .AppendConvention<InjectableConvention>()
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var a = () => ConventionContext.FromAsync(builder).AsTask();
        await a.ShouldThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldNotFailToConstructTheConventionInjectingTheValuesIfOptional()
    {
        var builder = ConventionContextBuilder
                     .Create(_ => [])
                     .AppendConvention<OptionalInjectableConvention>()
                     .Set<IConfiguration>(new ConfigurationBuilder().Build());
        var context = await ConventionContext.FromAsync(builder);
        var a = () => new ServiceCollection().ApplyConventionsAsync(context).AsTask();
        await a.ShouldNotThrowAsync();
    }

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
