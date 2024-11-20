using DryIoc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Serilog.Events;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocWebApplicationTests : AutoFakeTest<LocalTestContext>
{
    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc3>());
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<DryIocFixtures.IAbc>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc2>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc3>().Should().BeNull();
        sp.GetService<DryIocFixtures.IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) =>
                                                   {
                                                       container.Use(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       container.Use(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<DryIocFixtures.IAbc>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc2>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc3>().Should().BeNull();
        sp.GetService<DryIocFixtures.IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc3>());
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<DryIocFixtures.IAbc3>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) => { return container; }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .ConfigureDryIoc(
                                                   (context, configuration, services, container) => { return container; }
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<DryIocFixtures.IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_DryIoc()
    {
        await using var host = await WebApplication
                                    .CreateBuilder(Array.Empty<string>())
                                    .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var container = host.Services.GetRequiredService<IContainer>();
        container.Should().NotBeNull();
    }

    public DryIocWebApplicationTests(ITestOutputHelper outputHelper) : base(LocalTestContext.Create(outputHelper, LogEventLevel.Information))
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
    }
}
