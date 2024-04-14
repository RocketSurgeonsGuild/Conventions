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
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.Tests;

[ImportConventions]
public partial class RocketHostBuilderTests : AutoFakeTest
{
    [Fact]
    public void Should_UseAppDomain()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                  .UseAppDomain(AppDomain.CurrentDomain)
                           );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseAssemblies()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                  .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                           );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseRocketBooster()
    {
        var builder = Host.CreateDefaultBuilder()
                          .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain));

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseRocketBooster_With_Conventions()
    {
        var builder = Host.CreateDefaultBuilder()
                          .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain, GetConventions));

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseDiagnosticLogging()
    {
        var builder = Host.CreateDefaultBuilder()
                          .UseRocketBooster(
                               RocketBooster.For(AppDomain.CurrentDomain),
                               x => x.UseDiagnosticLogging(c => c.AddConsole())
                           );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseDependencyContext()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                  .UseDependencyContext(DependencyContext.Default!)
                           );

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

    [Fact]
    public void Should_ConfigureServices()
    {
        var convention = A.Fake<ServiceConvention>();
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDependencyContext(DependencyContext.Default!)
                                    .ConfigureServices(convention)
                           );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IServiceCollection>._)).MustHaveHappened();
    }

    [Fact]
    public void Should_ConfigureConfiguration()
    {
        var convention = A.Fake<ConfigurationConvention>();
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDependencyContext(DependencyContext.Default!)
                                    .ConfigureConfiguration(convention)
                           );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<IConfigurationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public void Should_ConfigureHosting()
    {
        var convention = A.Fake<HostingConvention>();
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDependencyContext(DependencyContext.Default!)
                                    .ConfigureHosting(convention)
                           );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IHostBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public void Should_ConfigureLogging()
    {
        var convention = A.Fake<LoggingConvention>();
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery(
                               rb => rb
                                    .UseDependencyContext(DependencyContext.Default!)
                                    .ConfigureLogging(convention)
                           );

        builder.Build();
        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IConfiguration>._, A<ILoggingBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public void Should_Build_The_Host_Correctly()
    {
        var builder = Host.CreateDefaultBuilder()
                          .ConfigureRocketSurgery();

        var host = builder.Build();
        host.Services.Should().NotBeNull();
    }

//    [Fact]
//    public async Task Should_Run_Rocket_CommandLine()
//    {
//        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
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
//        var builder = Host.CreateDefaultBuilder(new[] { "myself" })
//                          .ConfigureRocketSurgery(
//                               rb => rb
//                                    .AppendDelegate(new CommandLineConvention((a, c) => c.OnRun(state => 1337)))
//                                    .AppendDelegate(new CommandLineConvention((a, context) => context.AddCommand<MyCommand>("myself")))
//                           );
//
//        ( await builder.RunCli() ).Should().Be(1234);
//    }

    public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
}
