using Microsoft.Extensions.Logging;

using Rocket.Surgery.Extensions.Testing;

using Serilog.Events;

using Xunit.Abstractions;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.Conventions.Tests;

public class TestContextTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
{
    [Fact]
    public void Builder_Should_Create_Host()
    {
        var a = () => ConventionContextBuilder.Create(_ => []).UseLogger(Logger);
        var context = a.ShouldNotThrow();
        context.Get<ILogger>().ShouldBeSameAs(Logger);
    }

    [Fact]
    public void Builder_Should_Create_Host_ByType()
    {
        var a = () => ConventionContextBuilder.Create(_ => []).UseLogger(Logger);
        a.ShouldNotThrow();
    }

    [Fact]
    public void Builder_Should_Create_Host_ByAssembly()
    {
        var a = () => ConventionContextBuilder.Create(_ => []).UseLogger(Logger);
        a.ShouldNotThrow();
    }
}
