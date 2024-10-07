using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class TestContextTests : AutoFakeTest
{
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
        var a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory);
        a.Should().NotThrow();
    }

    [Fact]
    public void Builder_Should_Create_Host_ByAssembly()
    {
        var a = () => ConventionContextBuilder.Create().ForTesting(GetType().Assembly, LoggerFactory);
        a.Should().NotThrow();
    }

    [Fact]
    public void Builder_Should_Use_A_Custom_ILogger()
    {
        var a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory)
                                              .WithLogger(AutoFake.Resolve<ILogger>());
        var context = a.Should().NotThrow().Subject;
        context.Get<ILogger>().Should().BeSameAs(AutoFake.Resolve<ILogger>());
    }

    public TestContextTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information)
    {
    }
}
