using DryIoc;
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

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocWebApplicationTests : AutoFakeTest
{
    [Fact]
    public void ConstructTheContainerAndRegisterWithCore()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb.UseDryIoc()
                                                 .DisableConventionAttributes()
                                                 .ConfigureDryIoc(
                                                      (conventionContext, configuration, services, container) =>
                                                      {
                                                          container.RegisterInstance(A.Fake<DryIocFixtures.IAbc>());
                                                          services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                          return container;
                                                      }
                                                  )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc3>());
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefault).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefault).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefault).Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();

        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<DryIocFixtures.IAbc>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc2>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc3>().Should().BeNull();
        sp.GetService<DryIocFixtures.IAbc4>().Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) =>
                                                   {
                                                       container.Use(A.Fake<DryIocFixtures.IAbc>());
                                                       services.AddSingleton(A.Fake<DryIocFixtures.IAbc2>());
                                                       container.Use(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<DryIocFixtures.IAbc>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc2>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc3>().Should().BeNull();
        sp.GetService<DryIocFixtures.IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .DisableConventionAttributes()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) =>
                                                   {
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc3>());
                                                       container.RegisterInstance(A.Fake<DryIocFixtures.IAbc4>());
                                                       return container;
                                                   }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<DryIocFixtures.IAbc>().Should().BeNull();
        sp.GetService<DryIocFixtures.IAbc2>().Should().BeNull();
        sp.GetService<DryIocFixtures.IAbc3>().Should().NotBeNull();
        sp.GetService<DryIocFixtures.IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) => { return container; }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
        items.Resolve<DryIocFixtures.IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<DryIocFixtures.IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<DryIocFixtures.IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = WebApplication.CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseDryIoc()
                                              .ConfigureDryIoc(
                                                   (conventionContext, configuration, services, container) => { return container; }
                                               )
                                     );

        var items = builder.Build().Services.GetRequiredService<IResolverContext>();
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
        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                          .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        using var host = builder.Build();
        await host.StartAsync();
        var container = host.Services.GetRequiredService<IContainer>();
        container.Should().NotBeNull();
        await host.StopAsync();
    }

    public DryIocWebApplicationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        AutoFake.Provide<IDictionary<object, object?>>(new ServiceProviderDictionary());
    }
}
