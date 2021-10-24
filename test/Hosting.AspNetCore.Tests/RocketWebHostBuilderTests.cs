using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebHostBuilderTests : AutoFakeTest
{
    [Fact]
    public void Should_Build_The_Host_Correctly()
    {
        var serviceConventionFake = A.Fake<IServiceConvention>();
        var configurationConventionFake = A.Fake<IConfigurationConvention>();

        var builder = Host.CreateDefaultBuilder(Array.Empty<string>())
                          .ConfigureWebHostDefaults(x => x.UseStartup<TestStartup>().UseTestServer())
                          .ConfigureRocketSurgery(x => x.UseAssemblies(new[] { typeof(RocketWebHostBuilderTests).Assembly }));
        using (var host = builder.Build())
        {
            host.StartAsync();
            var server = host.GetTestServer();
            server.CreateClient();
            host.StopAsync();
        }
    }

    [Fact]
    public async Task Should_Run_The_Cli()
    {
        var serviceConventionFake = A.Fake<IServiceConvention>();
        var configurationConventionFake = A.Fake<IConfigurationConvention>();

        var builder = Host.CreateDefaultBuilder(new[] { "myself" })
                          .ConfigureWebHostDefaults(x => x.UseStartup<MyStartup>().UseTestServer())
                          .ConfigureRocketSurgery(
                               x => x.UseAssemblies(new[] { typeof(RocketWebHostBuilderTests).Assembly })
                                     .AppendDelegate(new CommandLineConvention((a, c) => c.OnRun(state => Task.FromResult(1337))))
                                     .AppendDelegate(
                                          new CommandLineConvention(
                                              (a, context) => context.AddCommand<MyCommand>("myself", v => { })
                                          )
                                      )
                           );

        var result = await builder.RunCli().ConfigureAwait(false);
        result.Should().Be(1234);
    }

    public RocketWebHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
}
