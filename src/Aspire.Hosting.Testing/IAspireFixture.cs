using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public interface IAspireFixture
{
    public DistributedApplication Aspire => App;
    DistributedApplication App { get; }

    IObservable<LogLine> GetResourceLogs();
    IObservable<ResourceEvent> GetResourceEvents();
}