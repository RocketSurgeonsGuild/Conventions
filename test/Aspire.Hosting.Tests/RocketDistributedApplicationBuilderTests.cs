using FakeItEasy;

using Microsoft.Extensions.Hosting;

using Rocket.Surgery.Aspire.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;

using Xunit.Abstractions;

namespace Aspire.Hosting.Tests;

public partial class RocketDistributedApplicationBuilderTests
    (ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public async Task Should_UseRocketBooster()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery();

        host.Services.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<DistributedApplicationConvention>();
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(rb => rb.ConfigureDistributedApplication(convention));

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
        host.Services.ShouldNotBeNull();
    }
}
