using Aspire.Hosting.Testing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Aspire.Hosting.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;
using RocketBooster = Rocket.Surgery.Aspire.Hosting.Testing.RocketBooster;

namespace Aspire.Hosting.Tests;

public partial class RocketDistributedApplicationTestingBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Should_UseAppDomain()
    {
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                            .UseAppDomain(AppDomain.CurrentDomain)
                                     );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseAssemblies()
    {
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                            .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                                     );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster()
    {
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .LaunchWith(RocketBooster.For(AppDomain.CurrentDomain));

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster_With_Conventions()
    {
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .UseRocketBooster(RocketBooster.For(Imports.Instance));

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseDiagnosticLogging()
    {
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .UseRocketBooster(
                                         RocketBooster.For(AppDomain.CurrentDomain),
                                         x => x.UseDiagnosticLogging(c => c.AddConsole())
                                     );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ConfigureServices()
    {
        var convention = A.Fake<ServiceConvention>();
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
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
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
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
        var convention = A.Fake<DistributedApplicationTestingConvention>();
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseConventionFactory(Imports.Instance)
                                              .ConfigureDistributedTestingApplication(convention)
                                     );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IDistributedApplicationTestingBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<Func<IHost, CancellationToken, ValueTask>>();
        var @delegate2 = A.Fake<Func<DistributedApplication, CancellationToken, ValueTask>>();
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<Projects.AspireSample>()
                                    .ConfigureRocketSurgery(z => z.OnHostCreated(@delegate).OnHostCreated(@delegate2));

        A.CallTo(() => @delegate.Invoke(A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => @delegate2.Invoke(A<DistributedApplication>._, A<CancellationToken>._)).MustHaveHappened();
        host.Services.Should().NotBeNull();
    }
}
