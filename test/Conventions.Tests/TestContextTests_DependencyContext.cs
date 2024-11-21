using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;
using Serilog.Events;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class TestContextTests_DependencyContext(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, LogEventLevel.Information))
{
    [field: AllowNull, MaybeNull]
    private ILoggerFactory LoggerFactory => field ??= CreateLoggerFactory();

    [Fact]
    public void Builder_Should_Create_Host()
    {
        var a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory);
        var context = a.Should().NotThrow().Subject;
        context.Get<ILoggerFactory>().Should().BeSameAs(LoggerFactory);
    }

    [Fact]
    public void Builder_Should_Create_Host_ByType()
    {
        var a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory)
            ;
        a.Should().NotThrow();
    }
}
