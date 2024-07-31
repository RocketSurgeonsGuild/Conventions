using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public interface ITestConvention : IConvention
{
    ValueTask Start(IProgramFixture fixture, IHost host);
    ValueTask Stop(IProgramFixture fixture, IHost host);
}