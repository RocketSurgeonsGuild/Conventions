namespace Rocket.Surgery.Clavus.Tests.Fixtures;

public interface ITestConvention : IClavusPart
{
    void Register(ITestClavusContext context);
}
