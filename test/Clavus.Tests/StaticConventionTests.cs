using Sample.DependencyThree.Clavus;

namespace Clavus.Tests;

public partial class StaticConventionTests
{
    [Test]
    public void Should_Have_Exports_Method_Defined()
    {
        var list = Exports
                  .GetConventions(ClavusContextBuilder.Create(_ => []))
                  .ShouldNotBeNull();
    }
}
