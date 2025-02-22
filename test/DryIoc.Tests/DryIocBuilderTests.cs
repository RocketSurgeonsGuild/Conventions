using DryIoc;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.DryIoc.Tests.DryIocFixtures;

#pragma warning disable CA1040, CA1034, CA2000, IDE0058, RCS1021

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocBuilderTests : AutoFakeTest<XUnitTestContext>
{
    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 container.RegisterInstance(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc3>());
                                                 container.RegisterInstance(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().ShouldNotBeNull();
        sp.GetService<IAbc2>().ShouldNotBeNull();
        sp.GetService<IAbc3>().ShouldBeNull();
        sp.GetService<IAbc4>().ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) =>
                                             {
                                                 container.Use(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 container.Use(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().ShouldNotBeNull();
        sp.GetService<IAbc2>().ShouldNotBeNull();
        sp.GetService<IAbc3>().ShouldBeNull();
        sp.GetService<IAbc4>().ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc3>());
                                                 container.RegisterInstance(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc3>().ShouldNotBeNull();
        sp.GetService<IAbc4>().ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) => { return container; }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .ConfigureDryIoc(
                                             (context, configuration, services, container) => { return container; }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_DryIoc()
    {
        using var host = await Host
                              .CreateApplicationBuilder(Array.Empty<string>())
                              .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var container = host.Services.GetRequiredService<IContainer>();
        container.ShouldNotBeNull();
    }

    public DryIocBuilderTests(ITestOutputHelper outputHelper) : base(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
    }

    protected override IContainer BuildContainer(IContainer container) =>
        container
           .With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic);
}
