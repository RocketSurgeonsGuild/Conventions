using Rocket.Surgery.Conventions;
using Sample.DependencyOne;

[assembly: Convention(typeof(Class1))]

namespace Sample.DependencyOne
{
    public class Class1 : IConvention
    {
    }
}
