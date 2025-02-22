using Rocket.Surgery.Conventions.Tests.Conventions;

namespace Rocket.Surgery.Conventions.Tests;

public partial class StaticConventionTests
{
    [Fact]
    public void Should_Have_Exports_Method_Defined()
    {
        var list = Exports
                  .GetConventions(ConventionContextBuilder.Create(_ => []))
                  .ShouldNotBeNull();
    }
}
