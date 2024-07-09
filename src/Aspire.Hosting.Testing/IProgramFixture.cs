using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public interface IProgramFixture
{
    IHost Host { get; }
    IServiceProvider Services { get; }
    Uri BaseAddress { get; }
    void SetLoggerFactory(ILoggerFactory loggerFactory);
    Task Reset();
    HttpClient CreateClient();
}