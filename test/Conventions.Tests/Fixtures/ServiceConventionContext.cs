namespace Rocket.Surgery.Conventions.Tests.Fixtures
{
    public interface IServiceConventionContext : IConventionContext
    {

    }

    public class ServiceConventionContext : ConventionContext, IServiceConventionContext
    {

    }
}
