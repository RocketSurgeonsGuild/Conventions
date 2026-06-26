using Aspire.Hosting.Testing;

using FakeItEasy;

using Microsoft.Extensions.Hosting;

using Projects;

using Rocket.Surgery.Aspire.Hosting.Testing;
using Rocket.Surgery.Extensions.Testing;

namespace Aspire.Hosting.Tests;

public partial class RocketDistributedApplicationTestingBuilderTests()
    : AutoFakeTest<TUnitTestRecord>(TUnitDefaults.CreateTestContext(TUnit.Core.TestContext.Current!))
{
    [Test]
    public async Task Should_UseRocketBooster()
    {
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<AspireSample>()
                                    .ConfigureRocketSurgery();

        host.Services.ShouldNotBeNull();
    }

    [Test]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<DistributedApplicationTestingConvention>();
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<AspireSample>()
                                    .ConfigureRocketSurgery(rb => rb.ConfigureDistributedTestingApplication(convention));

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IDistributedApplicationTestingBuilder>._)).MustHaveHappened();
    }

    [Test]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<Func<IHost, CancellationToken, ValueTask>>();
        var delegate2 = A.Fake<Func<DistributedApplication, CancellationToken, ValueTask>>();
        await using var host = await DistributedApplicationTestingBuilder
                                    .CreateAsync<AspireSample>()
                                    .ConfigureRocketSurgery(z => z.OnHostCreated(@delegate).OnHostCreated(delegate2));

        A.CallTo(() => @delegate.Invoke(A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => delegate2.Invoke(A<DistributedApplication>._, A<CancellationToken>._)).MustHaveHappened();
        host.Services.ShouldNotBeNull();
    }
}
