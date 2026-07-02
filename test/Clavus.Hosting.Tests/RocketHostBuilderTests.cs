using FakeItEasy;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Clavus.Configuration;
using Rocket.Surgery.Clavus.DependencyInjection;
using Rocket.Surgery.Clavus.Logging;
using Rocket.Surgery.Extensions.Testing;



namespace Rocket.Surgery.Clavus.Hosting.Tests;

public partial class RocketHostBuilderTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_UseRocketBooster_With_Conventions()
    {
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus();

        host.Services.ShouldNotBeNull();
    }

    [Test]
    public async Task Should_ConfigureServices()
    {
        var convention = A.Fake<ServicePart>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(rb => rb.ConfigureServices(convention));

        A.CallTo(() => convention.Invoke(A<IClavusContext>._, A<IConfiguration>._, A<IServiceCollection>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_ConfigureConfiguration()
    {
        var convention = A.Fake<ConfigurationPart>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(rb => rb.ConfigureConfiguration(convention));

        A.CallTo(() => convention.Invoke(A<IClavusContext>._, A<IConfiguration>._, A<IConfigurationBuilder>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<HostApplicationPart<IHostApplicationBuilder>>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(rb => rb.ConfigureApplication(convention));

        A.CallTo(() => convention.Invoke(A<IClavusContext>._, A<IHostApplicationBuilder>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_ConfigureHosting_HostApplication()
    {
        var convention = A.Fake<HostApplicationPart<HostApplicationBuilder>>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(rb => rb.ConfigureApplication(convention));

        A.CallTo(() => convention.Invoke(A<IClavusContext>._, A<HostApplicationBuilder>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_ConfigureLogging()
    {
        var convention = A.Fake<LoggingPart>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(rb => rb.ConfigureLogging(convention));

        A.CallTo(() => convention.Invoke(A<IClavusContext>._, A<IConfiguration>._, A<ILoggingBuilder>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<Func<IHost, CancellationToken, ValueTask>>();
        using var host = await Host
                              .CreateApplicationBuilder()
                              .ConfigureClavus(z => z.OnHostCreated(@delegate));

        A.CallTo(() => @delegate.Invoke(A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        host.Services.ShouldNotBeNull();
    }

    //    [Test]
    //    public async Task Should_Run_Rocket_CommandLine()
    //    {
    //        using var host = Host.CreateApplicationBuilder(Array.Empty<string>())
    //                          .ConfigureClavus(
    //                               rb => rb
    //                                  .AppendDelegate(
    //                                       new CommandLineConvention((a, c) => c.OnRun(state => 1337)),
    //                                       new CommandLineConvention((a, c) => c.OnRun(state => 1337))
    //                                   )
    //                           );
    //
    //        ( await builder.RunCli() ).ShouldBe(1337);
    //    }
    //
    //    [Test]
    //    public async Task Should_Inject_WebHost_Into_Command()
    //    {
    //        using var host = Host.CreateApplicationBuilder(new[] { "myself" })
    //                          .ConfigureClavus(
    //                               rb => rb
    //                                    .AppendDelegate(new CommandLineConvention((a, c) => c.OnRun(state => 1337)))
    //                                    .AppendDelegate(new CommandLineConvention((a, context) => context.AddCommand<MyCommand>("myself")))
    //                           );
    //
    //        ( await builder.RunCli() ).ShouldBe(1234);
    //    }
}
