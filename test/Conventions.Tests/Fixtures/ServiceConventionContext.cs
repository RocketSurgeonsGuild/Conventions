using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Tests.Fixtures
{
    public interface IServiceConventionContext : IConventionContext
    {

    }

    public class ServiceConventionContext : ConventionContext, IServiceConventionContext
    {
        public ServiceConventionContext(IConventionEnvironment environment, ILogger logger) : base(environment, logger, new Dictionary<object, object>())
        {
        }
    }
}
