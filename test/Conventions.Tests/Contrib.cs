using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;
using Rocket.Surgery.Conventions.Tests.Fixtures;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{

    internal class Contrib : IServiceConvention
    {
        public void Register(IServiceConventionContext context)
        {
        }
    }
}
