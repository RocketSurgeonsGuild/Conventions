namespace Rocket.Surgery.Conventions.Tests.Fixtures;

public interface ITestConvention : IConvention
{
    void Register(ITestConventionContext context);
}
