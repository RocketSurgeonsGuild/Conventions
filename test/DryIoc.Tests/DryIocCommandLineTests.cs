using System.Diagnostics;

using DryIoc;

using FakeItEasy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Testing;

using Serilog.Events;

using static Rocket.Surgery.Extensions.DryIoc.Tests.DryIocFixtures;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocCommandLineTests() : AutoFakeTest<TUnitTestRecord>(TUnitDefaults.CreateTestContext(TUnit.Core.TestContext.Current!, LogEventLevel.Information))
{
    [Before(Test)]
    public void Setup() => AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));

    [Test]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDryIoc()
                                     .ConfigureDryIoc(
                                          (context, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = await Host
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
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDryIoc()
                                     .ConfigureDryIoc(
                                          (context, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc3>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDryIoc()
                                     .ConfigureDryIoc(
                                          (context, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().ShouldNotBeNull();
        sp.GetService<IAbc2>().ShouldNotBeNull();
        sp.GetService<IAbc3>().ShouldBeNull();
        sp.GetService<IAbc4>().ShouldBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = await Host
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
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().ShouldNotBeNull();
        sp.GetService<IAbc2>().ShouldNotBeNull();
        sp.GetService<IAbc3>().ShouldBeNull();
        sp.GetService<IAbc4>().ShouldNotBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDryIoc()
                                     .ConfigureDryIoc(
                                          (context, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc3>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc3>().ShouldNotBeNull();
        sp.GetService<IAbc4>().ShouldNotBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).ShouldNotBeNull();
    }

    [Test]
    public async Task Should_Integrate_With_DryIoc()
    {
        using var host = await Host
                              .CreateApplicationBuilder([])
                              .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        host.Services.GetRequiredService<IContainer>().ShouldNotBeNull();
    }

}
