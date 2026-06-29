using Sample.DependencyThree.Conventions;

namespace Rocket.Surgery.Conventions.Tests;

public partial class StaticConventionTests
{
    [Test]
    public void Should_Have_Exports_Method_Defined()
    {
        var list = Exports
                  .GetConventions(ConventionContextBuilder.Create(_ => []))
                  .ShouldNotBeNull();
    }
}
