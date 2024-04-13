using System.Diagnostics;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.Autofac.Tests.AutofacFixtures;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

public class AutofacWebApplicationTests : AutoFakeTest
{
    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .DisableConventionAttributes()
                                     .ConfigureAutofac(
                                          (conventionContext, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .DisableConventionAttributes()
                                     .ConfigureAutofac(
                                          (conventionContext, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .DisableConventionAttributes()
                                     .ConfigureAutofac(
                                          (conventionContext, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc3>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc3>().Should().NotBeNull();
        items.ResolveOptional<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .DisableConventionAttributes()
                                     .ConfigureAutofac(
                                          (conventionContext, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .DisableConventionAttributes()
                                     .ConfigureAutofac(
                                          (conventionContext, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc>());
                                              services.AddSingleton(A.Fake<IAbc2>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseAutofac()
                                     .DisableConventionAttributes()
                                     .ConfigureAutofac(
                                          (conventionContext, configuration, services, container) =>
                                          {
                                              container.RegisterInstance(A.Fake<IAbc3>());
                                              container.RegisterInstance(A.Fake<IAbc4>());
                                          }
                                      )
                            );

        var items = builder.GetLifetimeScope();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                   .UseAutofac()
                            );

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = await WebApplication
                           .CreateBuilder()
                           .ConfigureRocketSurgery(rb => rb.UseAutofac());

        var items = builder.GetLifetimeScope();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().BeNull();
        items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
        items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Integrate_With_Autofac()
    {
        var builder = await Host
                           .CreateApplicationBuilder(Array.Empty<string>())
                           .ConfigureRocketSurgery(rb => rb.UseAutofac());

        builder.GetLifetimeScope().Should().NotBeNull();
    }

    public AutofacWebApplicationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
    }
}