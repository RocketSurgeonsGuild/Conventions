using DryIoc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.DryIoc.Tests.DryIocFixtures;

#pragma warning disable CA1040, CA1034, CA2000, IDE0058, RCS1021

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocBuilderTests : AutoFakeTest
{
    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .DisableConventionAttributes()
                                        .ConfigureDryIoc(
                                             (conventionContext, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .DisableConventionAttributes()
                                        .ConfigureDryIoc(
                                             (conventionContext, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 container.RegisterInstance(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .DisableConventionAttributes()
                                        .ConfigureDryIoc(
                                             (conventionContext, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc3>());
                                                 container.RegisterInstance(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .DisableConventionAttributes()
                                        .ConfigureDryIoc(
                                             (conventionContext, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc>());
                                                 services.AddSingleton(A.Fake<IAbc2>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .DisableConventionAttributes()
                                        .ConfigureDryIoc(
                                             (conventionContext, configuration, services, container) =>
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
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseDryIoc()
                                        .DisableConventionAttributes()
                                        .ConfigureDryIoc(
                                             (conventionContext, configuration, services, container) =>
                                             {
                                                 container.RegisterInstance(A.Fake<IAbc3>());
                                                 container.RegisterInstance(A.Fake<IAbc4>());
                                                 return container;
                                             }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
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
                                             (conventionContext, configuration, services, container) => { return container; }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
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
                                             (conventionContext, configuration, services, container) => { return container; }
                                         )
                               );

        var items = host.Services.GetRequiredService<IResolverContext>();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_DryIoc()
    {
        using var host = await Host
                              .CreateApplicationBuilder(Array.Empty<string>())
                              .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        await host.StartAsync();
        var container = host.Services.GetRequiredService<IContainer>();
        container.Should().NotBeNull();
        await host.StopAsync();
    }

    public DryIocBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
    }
}