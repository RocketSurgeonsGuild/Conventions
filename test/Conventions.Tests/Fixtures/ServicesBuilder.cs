using System.Collections.Generic;
using Rocket.Surgery.Builders;

namespace Rocket.Surgery.Conventions.Tests.Fixtures
{
    public class ServicesBuilder : Builder
    {
        protected ServicesBuilder() : base(new Dictionary<object, object>())
        {
        }
    }
}
