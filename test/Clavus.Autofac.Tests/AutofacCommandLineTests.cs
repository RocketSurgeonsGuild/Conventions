using System.Diagnostics;
using Autofac;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Testing;
using static Clavus.Autofac.Tests.AutofacFixtures;

namespace Clavus.Autofac.Tests;

public class AutofacCommandLineTests : AutoFakeTest<TestRecord>
{
    [Test]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureClavus(
                                rb => rb
                                     .ConfigureAutofac((context, container) => container.RegisterInstance(A.Fake<IAbc>()))
                                     .ConfigureServices((context, services) => services.AddSingleton(A.Fake<IAbc2>()))
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureClavus(
                                rb => rb
                                     .ConfigureAutofac((context, container) =>
                                                       {
                                                           container.RegisterInstance(A.Fake<IAbc>());
                                                           container.RegisterInstance(A.Fake<IAbc4>());
                                                       }
                                      )
                                     .ConfigureServices((context, services) => services.AddSingleton(A.Fake<IAbc2>()))
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldNotBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureClavus(
                                rb => rb
                                     .ConfigureAutofac(
                                          (context, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc3>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc3>().ShouldNotBeNull();
        items.ResolveOptional<IAbc4>().ShouldNotBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureClavus(
                                rb => rb
                                     .ConfigureAutofac(
                                          (context, container) => container.RegisterInstance(A.Fake<IAbc>())
                                      )
                                     .ConfigureServices((context, services) => services.AddSingleton(A.Fake<IAbc2>()))
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
                           .ConfigureClavus(
                                rb => rb
                                     .ConfigureAutofac(
                                          (context, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                                        .ConfigureServices((context, services) => services.AddSingleton(A.Fake<IAbc2>()))
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
                           .ConfigureClavus(
                                rb => rb
                                     .ConfigureAutofac(
                                          (context, container) =>
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
                           .ConfigureClavus();

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldBeNull();
    }

    [Test]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureClavus();

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldBeNull();
        items.ResolveOptional<IOtherAbc3>().ShouldNotBeNull();
        items.ResolveOptional<IOtherAbc3>().ShouldNotBeNull();
    }

    [Test]
    public async Task Should_Integrate_With_Autofac()
    {
        using var host = await Host
                              .CreateApplicationBuilder([])
                              .ConfigureClavus();

        host.Services.GetRequiredService<ILifetimeScope>().ShouldNotBeNull();
    }

    public AutofacCommandLineTests() : base(TestRecord.Create()) => AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
}
