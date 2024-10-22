using FluentAssertions;
using Rocket.Surgery.Conventions.Tests.Conventions;

namespace Rocket.Surgery.Conventions.Tests;

public partial class StaticConventionTests
{
    [Fact]
    public void Should_Have_Exports_Method_Defined()
    {
        var list = Exports
                  .GetConventions(new(new Dictionary<object, object>(), []))
                  .Should()
                  .NotBeNull()
                  .And.Subject;
        list.As<IEnumerable<IConventionMetadata>>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Assembly()
    {
        var list = Imports
                  .Instance.LoadConventions(new(new Dictionary<object, object>(), []))
                  .Should()
                  .NotBeNull()
                  .And.Subject;
        list.As<IEnumerable<IConventionMetadata>>().Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Imports_Method_Defined_On_Assembly_Into_Provider()
    {
        var list = Exports
                  .GetConventions(new(new Dictionary<object, object>(), []))
                  .Should()
                  .NotBeNull()
                  .And.Subject;

        var items = list.As<IEnumerable<IConventionMetadata>>().Should().NotBeNull().And.Subject;

        var provider = new ConventionProvider(HostType.Undefined, [], items.Cast<object>().ToList());
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
                  .LoadConventions(new(new Dictionary<object, object>(), []))
                  .Should()
                  .NotBeNull()
                  .And.Subject;

        list.As<IEnumerable<IConventionMetadata>>().Should().NotBeNull();
    }
}
