using Rocket.Surgery.Extensions.Testing;

using Serilog.Events;


using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.Conventions.Tests;

public class TestContextTests_DependencyContext
    () : AutoFakeTest<TUnitTestRecord>(TUnitDefaults.Create(LogEventLevel.Information))
{
    [Test]
    public void Builder_Should_Create_Host()
    {
        var a = () => ConventionContextBuilder.Create(_ => []).UseLogger(Logger);
        var context = a.ShouldNotThrow();
        context.Get<ILogger>().ShouldBeSameAs(Logger);
    }

    [Test]
    public void Builder_Should_Create_Host_ByType()
    {
        var a = () => ConventionContextBuilder.Create(_ => []).UseLogger(Logger)
            ;
        a.ShouldNotThrow();
    }
}
