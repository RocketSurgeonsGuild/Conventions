#pragma warning disable CA1040, CA1034, CA2000, IDE0058, RCS1021
using System.Diagnostics;
using Autofac;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.Autofac.Tests.AutofacFixtures;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

public class AutofacBuilderTests : AutoFakeTest<XUnitTestContext>
{
    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .ConfigureAutofac(
                                          (context, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .ConfigureAutofac(
                                          (context, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldNotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .ConfigureAutofac(
                                          (context, configuration, services, container) =>
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

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .ConfigureAutofac(
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

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .ConfigureAutofac(
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

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .ConfigureAutofac(
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

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                   .UseAutofac()
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(rb => rb.UseAutofac());

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().ShouldNotBeNull();
        items.ResolveOptional<IAbc2>().ShouldNotBeNull();
        items.ResolveOptional<IAbc3>().ShouldBeNull();
        items.ResolveOptional<IAbc4>().ShouldBeNull();
        items.ResolveOptional<IOtherAbc3>().ShouldNotBeNull();
        items.ResolveOptional<IOtherAbc3>().ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_Autofac()
    {
        using var host = await Host
                              .CreateApplicationBuilder(Array.Empty<string>())
                              .ConfigureRocketSurgery(rb => rb.UseAutofac());

        host.Services.GetRequiredService<ILifetimeScope>().ShouldNotBeNull();
    }

    public AutofacBuilderTests(ITestOutputHelper outputHelper) : base(XUnitTestContext.Create(outputHelper))
    {
        AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
    }
}
