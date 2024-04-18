using FluentAssertions;
using Rocket.Surgery.Conventions.Tests.Conventions;

namespace Rocket.Surgery.Conventions.Tests;

public partial class StaticConventionTests
{
    [Fact]
    public void Should_Have_Exports_Method_Defined()
    {
        var list = Exports
                  .GetConventions(new(new Dictionary<object, object>()))
                  .Should()
                  .NotBeNull()
                  .And.Subject;
        list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Assembly()
    {
        var list = Imports
                  .Instance.LoadConventions(new(new Dictionary<object, object>()))
                  .Should()
                  .NotBeNull()
                  .And.Subject;
        list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Assembly_Into_Provider()
    {
        var list = Exports
                  .GetConventions(new(new Dictionary<object, object>()))
                  .Should()
                  .NotBeNull()
                  .And.Subject;

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
        var list = Imports
                  .Instance
                  .LoadConventions(new(new Dictionary<object, object>()))
                  .Should()
                  .NotBeNull()
                  .And.Subject;

        list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
    }
}
