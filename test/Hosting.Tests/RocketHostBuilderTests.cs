using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.Reflection;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.Tests;

public partial class RocketHostBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Should_UseAppDomain()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                      .UseAppDomain(AppDomain.CurrentDomain)
                               );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseAssemblies()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                      .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                               );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .UseRocketBooster(ReflectionRocketBooster.For(AppDomain.CurrentDomain));

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster_With_Conventions()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .UseRocketBooster(RocketBooster.For(Imports.Instance));

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseDiagnosticLogging()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .UseRocketBooster(
                                   ReflectionRocketBooster.For(AppDomain.CurrentDomain),
                                   x => x.UseDiagnosticLogging(c => c.AddConsole())
                               );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseDependencyContext()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb.UseConventionFactory(Imports.Instance)
                               );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ConfigureServices()
    {
        var convention = A.Fake<ServiceConvention>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseConventionFactory(Imports.Instance)
                                        .ConfigureServices(convention)
                               );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IServiceCollection>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureConfiguration()
    {
        var convention = A.Fake<ConfigurationConvention>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseConventionFactory(Imports.Instance)
                                        .ConfigureConfiguration(convention)
                               );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IConfigurationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<HostApplicationConvention<IHostApplicationBuilder>>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseConventionFactory(Imports.Instance)
                                        .ConfigureApplication(convention)
                               );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IHostApplicationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureHosting_HostApplication()
    {
        var convention = A.Fake<HostApplicationConvention<HostApplicationBuilder>>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseConventionFactory(Imports.Instance)
                                        .ConfigureApplication(convention)
                               );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<HostApplicationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureLogging()
    {
        var convention = A.Fake<LoggingConvention>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(
                                   rb => rb
                                        .UseConventionFactory(Imports.Instance)
                                        .ConfigureLogging(convention)
                               );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<ILoggingBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<Func<IHost, CancellationToken, ValueTask>>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureRocketSurgery(z => z.OnHostCreated(@delegate));

        A.CallTo(() => @delegate.Invoke(A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        host.Services.Should().NotBeNull();
    }

//    [Fact]
//    public async Task Should_Run_Rocket_CommandLine()
//    {
//        using var host = Host.CreateApplicationBuilder(Array.Empty<string>())
//                          .ConfigureRocketSurgery(
//                               rb => rb
//                                  .AppendDelegate(
//                                       new CommandLineConvention((a, c) => c.OnRun(state => 1337)),
//                                       new CommandLineConvention((a, c) => c.OnRun(state => 1337))
//                                   )
//                           );
//
//        ( await builder.RunCli() ).Should().Be(1337);
//    }
//
//    [Fact]
//    public async Task Should_Inject_WebHost_Into_Command()
//    {
//        using var host = Host.CreateApplicationBuilder(new[] { "myself" })
//                          .ConfigureRocketSurgery(
//                               rb => rb
//                                    .AppendDelegate(new CommandLineConvention((a, c) => c.OnRun(state => 1337)))
//                                    .AppendDelegate(new CommandLineConvention((a, context) => context.AddCommand<MyCommand>("myself")))
//                           );
//
//        ( await builder.RunCli() ).Should().Be(1234);
//    }
}
