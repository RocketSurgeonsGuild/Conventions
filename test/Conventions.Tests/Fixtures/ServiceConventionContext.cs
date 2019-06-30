using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Tests.Fixtures
{
    public class ServiceConventionContext : ConventionContext, IServiceConventionContext
    {
        public ServiceConventionContext(ILogger logger) : base(logger, new Dictionary<object, object>())
        {
        }
    }
}
