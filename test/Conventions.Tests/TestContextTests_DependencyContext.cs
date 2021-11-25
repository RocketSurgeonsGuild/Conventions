using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;
using Rocket.Surgery.Conventions.Testing;

namespace Rocket.Surgery.Conventions.Tests;

public class TestContextTests_DependencyContext : AutoFakeTest
{
    [Fact]
    public void Builder_Should_Create_Host()
    {
        Func<ConventionContextBuilder> a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory);
        var context = a.Should().NotThrow().Subject;
        context.Get<ILoggerFactory>().Should().BeSameAs(LoggerFactory);
    }

    [Fact]
    public void Builder_Should_Create_Host_ByType()
    {
        Func<ConventionContextBuilder> a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory)
                                 ;
        a.Should().NotThrow();
    }

    [Fact]
    public void Builder_Should_Create_Host_ByAssembly()
    {
        Func<ConventionContextBuilder> a = () => ConventionContextBuilder.Create().ForTesting(GetType().Assembly, LoggerFactory)
                                 ;
        a.Should().NotThrow();
    }

    [Fact]
    public void Builder_Should_Use_A_Custom_ILogger()
    {
        Func<ConventionContextBuilder> a = () => ConventionContextBuilder.Create().ForTesting(GetType(), LoggerFactory)
                                 .WithLogger(AutoFake.Resolve<ILogger>())
                                 ;
        var context = a.Should().NotThrow().Subject;
        context.Get<ILogger>().Should().BeSameAs(AutoFake.Resolve<ILogger>());
    }

    public TestContextTests_DependencyContext(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information)
    {
    }
}
