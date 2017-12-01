using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Tests.Fixtures
{
    public interface IServiceConventionContext : IConventionContext
    {

    }

    public class ServiceConventionContext : ConventionContext, IServiceConventionContext
    {
        public ServiceConventionContext(ILogger logger) : base(logger)
        {
        }
    }
}
