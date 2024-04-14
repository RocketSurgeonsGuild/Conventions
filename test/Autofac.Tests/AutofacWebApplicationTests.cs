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
using Rocket.Surgery.Web.Hosting;
using Xunit;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.Autofac.Tests.AutofacFixtures;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

public class AutofacWebApplicationTests : AutoFakeTest
{
    [Fact]
    public void ConstructTheContainerAndRegisterWithCore()
    {
        var builder = WebApplication.CreateBuilder()
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

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = WebApplication.CreateBuilder()
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

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = WebApplication.CreateBuilder()
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

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        items.ResolveOptional<IAbc>().Should().BeNull();
        items.ResolveOptional<IAbc2>().Should().BeNull();
        items.ResolveOptional<IAbc3>().Should().NotBeNull();
        items.ResolveOptional<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = WebApplication.CreateBuilder()
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

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = WebApplication.CreateBuilder()
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

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = WebApplication.CreateBuilder()
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

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().BeNull();
        sp.GetService<IAbc2>().Should().BeNull();
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                            .UseAutofac()
                                     );

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(rb => rb.UseAutofac());

        var items = builder.Build().Services.GetRequiredService<ILifetimeScope>();
        items.ResolveOptional<IAbc>().Should().NotBeNull();
        items.ResolveOptional<IAbc2>().Should().NotBeNull();
        items.ResolveOptional<IAbc3>().Should().BeNull();
        items.ResolveOptional<IAbc4>().Should().BeNull();
        items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
        items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Integrate_With_Autofac()
    {
        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                          .ConfigureRocketSurgery(rb => rb.UseAutofac());

        using var host = builder.Build();
        host.Services.GetRequiredService<ILifetimeScope>().Should().NotBeNull();
    }

    public AutofacWebApplicationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
    }
}
