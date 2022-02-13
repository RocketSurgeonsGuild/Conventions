using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests;

[ImportConventions]
public partial class StaticConventionTests
{
    [Fact]
    public void Should_Have_Exports_Method_Defined()
    {
        var list = Conventions.Exports.GetConventions(new ServiceCollection().BuildServiceProvider())
                              .Should().NotBeNull().And.Subject;
        list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Assembly()
    {
        var list = Imports.GetConventions(new ServiceCollection().BuildServiceProvider())
                          .Should().NotBeNull().And.Subject;
        list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Assembly_Into_Provider()
    {
        var list = Conventions.Exports.GetConventions(new ServiceCollection().BuildServiceProvider())
                              .Should().NotBeNull().And.Subject;

        var items = list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull().And.Subject;

        var provider = new ConventionProvider(HostType.Undefined, items, Array.Empty<object>(), Array.Empty<object>());
        var a = () => provider.GetAll();

        a.Should().NotThrow();

        var values = a().ToArray();
        values.OfType<Contrib>().Should().HaveCount(1);
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Class()
    {
        var list = GetConventions(new ServiceCollection().BuildServiceProvider())
                  .Should().NotBeNull().And.Subject;

        list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
    }
}
