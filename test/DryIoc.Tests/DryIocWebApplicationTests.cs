using DryIoc;

using FakeItEasy;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;

using Serilog.Events;

using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocWebApplicationTests : AutoFakeTest<XUnitTestContext>
{
    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
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
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).ShouldBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
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
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).ShouldBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
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
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
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
        sp.GetService<DryIocFixtures.IAbc>().ShouldNotBeNull();
        sp.GetService<DryIocFixtures.IAbc2>().ShouldNotBeNull();
        sp.GetService<DryIocFixtures.IAbc3>().ShouldBeNull();
        sp.GetService<DryIocFixtures.IAbc4>().ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
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
        sp.GetService<DryIocFixtures.IAbc>().ShouldNotBeNull();
        sp.GetService<DryIocFixtures.IAbc2>().ShouldNotBeNull();
        sp.GetService<DryIocFixtures.IAbc3>().ShouldBeNull();
        sp.GetService<DryIocFixtures.IAbc4>().ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
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
        sp.GetService<DryIocFixtures.IAbc3>().ShouldNotBeNull();
        sp.GetService<DryIocFixtures.IAbc4>().ShouldNotBeNull();
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
                                                   (context, configuration, services, container) => container
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
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
                                                   (context, configuration, services, container) => container
                                               )
                                     );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<DryIocFixtures.IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<DryIocFixtures.IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_DryIoc()
    {
        await using var host = await WebApplication
                                    .CreateBuilder([])
                                    .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var container = host.Services.GetRequiredService<IContainer>();
        container.ShouldNotBeNull();
    }

    public DryIocWebApplicationTests(ITestOutputHelper outputHelper) : base(XUnitTestContext.Create(outputHelper, LogEventLevel.Information)) =>
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
}
