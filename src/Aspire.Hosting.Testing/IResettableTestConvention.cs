using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public interface IResettableTestConvention : ITestConvention
{
    ValueTask Reset(IProgramFixture fixture, IHost serviceProvider);
}