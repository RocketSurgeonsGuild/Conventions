using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.Tests;

public partial class RocketHostBuilderTests : AutoFakeTest
{
    [Fact]
    public async Task Should_UseAppDomain()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                   .UseAppDomain(AppDomain.CurrentDomain)
                            );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseAssemblies()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                   .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                            );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain));

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster_With_Conventions()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .UseRocketBooster(RocketBooster.For(Imports.GetConventions));

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseDiagnosticLogging()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .UseRocketBooster(
                                RocketBooster.For(AppDomain.CurrentDomain),
                                x => x.UseDiagnosticLogging(c => c.AddConsole())
                            );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseDependencyContext()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                   .UseDependencyContext(DependencyContext.Default!)
                            );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ConfigureServices()
    {
        var convention = A.Fake<ServiceConvention>();
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDependencyContext(DependencyContext.Default!)
                                     .ConfigureServices(convention)
                            );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IServiceCollection>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureConfiguration()
    {
        var convention = A.Fake<ConfigurationConvention>();
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDependencyContext(DependencyContext.Default!)
                                     .ConfigureConfiguration(convention)
                            );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IConfigurationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<HostApplicationConvention>();
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDependencyContext(DependencyContext.Default!)
                                     .ConfigureApplication(convention)
                            );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IHostApplicationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureLogging()
    {
        var convention = A.Fake<LoggingConvention>();
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery(
                                rb => rb
                                     .UseDependencyContext(DependencyContext.Default!)
                                     .ConfigureLogging(convention)
                            );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<ILoggingBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var builder = await Host
                           .CreateApplicationBuilder()
                           .ConfigureRocketSurgery();

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

//    [Fact]
//    public async Task Should_Run_Rocket_CommandLine()
//    {
//        var builder = Host.CreateApplicationBuilder(Array.Empty<string>())
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
//        var builder = Host.CreateApplicationBuilder(new[] { "myself" })
//                          .ConfigureRocketSurgery(
//                               rb => rb
//                                    .AppendDelegate(new CommandLineConvention((a, c) => c.OnRun(state => 1337)))
//                                    .AppendDelegate(new CommandLineConvention((a, context) => context.AddCommand<MyCommand>("myself")))
//                           );
//
//        ( await builder.RunCli() ).Should().Be(1234);
//    }

    public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
}