using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Aspire.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Aspire.Hosting.Tests;

public partial class RocketDistributedApplicationBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Should_UseAppDomain()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                            .UseAppDomain(AppDomain.CurrentDomain)
                                     );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseAssemblies()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                            .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                                     );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain));

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseRocketBooster_With_Conventions()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .UseRocketBooster(RocketBooster.For(Imports.Instance));

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_UseDiagnosticLogging()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .UseRocketBooster(
                                         RocketBooster.For(AppDomain.CurrentDomain),
                                         x => x.UseDiagnosticLogging(c => c.AddConsole())
                                     );

        host.Services.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<DistributedApplicationConvention>();
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                              .UseConventionFactory(Imports.Instance)
                                              .ConfigureDistributedApplication(convention)
                                     );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IDistributedApplicationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<Func<IHost, CancellationToken, ValueTask>>();
        var delegate2 = A.Fake<Func<DistributedApplication, CancellationToken, ValueTask>>();
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(z => z.OnHostCreated(@delegate).OnHostCreated(delegate2));

        A.CallTo(() => @delegate.Invoke(A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => delegate2.Invoke(A<DistributedApplication>._, A<CancellationToken>._)).MustHaveHappened();
        host.Services.Should().NotBeNull();
    }
}