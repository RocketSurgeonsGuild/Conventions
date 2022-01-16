using System.Diagnostics;
using DryIoc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.DryIoc.Tests.DryIocFixtures;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocCommandLineTests : AutoFakeTest
{
    [Fact]
    public void ConstructTheContainerAndRegisterWithCore()
    {
        var builder = App.Create(
            rb => rb.UseDryIoc()
                    .DisableConventionAttributes()
                    .ConfigureDryIoc(
                         (conventionContext, configuration, services, container) =>
                         {
                             container.RegisterInstance(A.Fake<IAbc>());
                             services.AddSingleton(A.Fake<IAbc2>());
                         }
                     )
        );

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithApplication()
    {
        var builder = App.Create(
            rb => rb
                 .UseDryIoc()
                 .DisableConventionAttributes()
                 .ConfigureDryIoc(
                      (conventionContext, configuration, services, container) =>
                      {
                          container.RegisterInstance(A.Fake<IAbc>());
                          services.AddSingleton(A.Fake<IAbc2>());
                          container.RegisterInstance(A.Fake<IAbc4>());
                      }
                  )
        );

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem()
    {
        var builder = App.Create(
            rb => rb
                 .UseDryIoc()
                 .DisableConventionAttributes()
                 .ConfigureDryIoc(
                      (conventionContext, configuration, services, container) =>
                      {
                          container.RegisterInstance(A.Fake<IAbc3>());
                          container.RegisterInstance(A.Fake<IAbc4>());
                      }
                  )
        );

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
    {
        var builder = App.Create(
            rb => rb
                 .UseDryIoc()
                 .DisableConventionAttributes()
                 .ConfigureDryIoc(
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
    public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
    {
        var builder = App.Create(
            rb => rb
                 .UseDryIoc()
                 .DisableConventionAttributes()
                 .ConfigureDryIoc(
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
    public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
    {
        var builder = App.Create(
            rb => rb
                 .UseDryIoc()
                 .DisableConventionAttributes()
                 .ConfigureDryIoc(
                      (conventionContext, configuration, services, container) =>
                      {
                          container.RegisterInstance(A.Fake<IAbc3>());
                          container.RegisterInstance(A.Fake<IAbc4>());
                      }
                  )
        );

        var items = builder.GetLifetimeScope();
        var sp = items.Resolve<IServiceProvider>();
        sp.GetService<IAbc>().Should().BeNull();
        sp.GetService<IAbc2>().Should().BeNull();
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = App.Create(rb => rb.UseDryIoc());

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
    public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = App.Create(rb => rb.UseDryIoc());

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IOtherAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
    public void Should_Integrate_With_DryIoc()
    {
        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                          .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        using var host = builder.Build();
        host.Services.GetRequiredService<IContainer>().Should().NotBeNull();
    }

    public DryIocCommandLineTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
    }
}
