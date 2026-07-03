using Rocket.Surgery.Extensions.Testing;

using Serilog.Events;


using ILogger = Serilog.ILogger;

namespace Clavus.Tests;

public class TestContextTests() : AutoFakeTest<TestRecord>(TestRecord.Create(LogEventLevel.Information))
{
    [Test]
    public void Builder_Should_Create_Host()
    {
        var a = () => ClavusContextBuilder.Create(_ => []).Set(Logger);
        var context = a.ShouldNotThrow();
        context.Get<ILogger>().ShouldBeSameAs(Logger);
    }

    [Test]
    public void Builder_Should_Create_Host_ByType()
    {
        var a = () => ClavusContextBuilder.Create(_ => []).Set(Logger);
        a.ShouldNotThrow();
    }

    [Test]
    public void Builder_Should_Create_Host_ByAssembly()
    {
        var a = () => ClavusContextBuilder.Create(_ => []).Set(Logger);
        a.ShouldNotThrow();
    }
}
