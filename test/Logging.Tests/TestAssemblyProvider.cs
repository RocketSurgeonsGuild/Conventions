using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Logging.Tests
{
    class TestAssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return new[]
            {
                typeof(LoggingBuilder).GetTypeInfo().Assembly,
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly
            };
        }
    }
}
