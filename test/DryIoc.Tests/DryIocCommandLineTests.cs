﻿using System.Diagnostics;
using DryIoc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using static Rocket.Surgery.Extensions.DryIoc.Tests.DryIocFixtures;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public class DryIocCommandLineTests : AutoFakeTest<XUnitTestContext>
{
    [Fact]
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
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
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
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
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
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
    }

    [Fact]
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
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().BeNull();
    }

    [Fact]
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
        sp.GetService<IAbc>().Should().NotBeNull();
        sp.GetService<IAbc2>().Should().NotBeNull();
        sp.GetService<IAbc3>().Should().BeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
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
        sp.GetService<IAbc3>().Should().NotBeNull();
        sp.GetService<IAbc4>().Should().NotBeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var items = builder.GetLifetimeScope();
        items.Resolve<IAbc>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc2>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().NotBeNull();
        items.Resolve<IAbc3>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
        items.Resolve<IAbc4>(IfUnresolved.ReturnDefaultIfNotRegistered).Should().BeNull();
    }

    [Fact]
    public async Task ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(rb => rb.UseDryIoc());

        var items = builder.GetLifetimeScope();
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

        host.Services.GetRequiredService<IContainer>().Should().NotBeNull();
    }

    public DryIocCommandLineTests(ITestOutputHelper outputHelper) : base(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
    {
        AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
    }
}
