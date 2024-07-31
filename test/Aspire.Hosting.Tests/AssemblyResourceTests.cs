using Aspirant.Hosting.Testing;
using Aspire.Hosting.Testing;
using Projects;
using Rocket.Surgery.Aspire.Hosting.Testing;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Aspire.Hosting.Tests;

public class AssemblyResourceTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
{
    [Fact]
    public async Task Should_Do_Things()
    {
        var testBuilder = await DistributedApplicationTestingBuilder.CreateAsync<AspireSample>();

        testBuilder
           .FixContentRoot()
           .RunInMemory<Projects.Sample, Program>();

        testBuilder.Services.AddResourceWatching();


        var app = await testBuilder.BuildAsync();


        app.ObserveResourceLogs().Subscribe(z => outputHelper.WriteLine(z.Content));
        app.ObserveResourceEvents().Subscribe(z => outputHelper.WriteLine(z.Snapshot.ToString()));

        var sample = app.GetAssemblyResource("sample");
        sample.ObserveLogs().Subscribe(z => outputHelper.WriteLine(z.Content));

        await app.StartAsync(true);

        await Task.Delay(5000);

        await app.StopAsync();

        await Verify(sample.Logs);

        throw new();
    }
}