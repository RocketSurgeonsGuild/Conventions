using Aspire.Hosting;
using Clavus.Hosting;
using FakeItEasy;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Testing;

namespace Clavus.Aspire.Tests;

public partial class RocketDistributedApplicationBuilderTests
    () : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_UseRocketBooster()
    {
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureClavus();

        host.Services.ShouldNotBeNull();
    }

    [Test]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<DistributedApplicationBuilderPart>();
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureClavus(rb => rb.ConfigureDistributedApplicationBuilder(convention));

        A.CallTo(() => convention.Invoke(A<IClavusContext>._, A<IDistributedApplicationBuilder>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<HostCreatedAsyncPart<IHost>>();
        var delegate2 = A.Fake<HostCreatedPart<DistributedApplication>>();
        await using var host = await DistributedApplication
                                    .CreateBuilder()
                                    .ConfigureClavus(z => z.ConfigureHostCreated(@delegate).ConfigureHostCreated(delegate2));

        A.CallTo(() => @delegate.Invoke(A<IClavusContext>._, A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => delegate2.Invoke(A<IClavusContext>._, A<DistributedApplication>._)).MustHaveHappened();
        host.Services.ShouldNotBeNull();
    }
}
