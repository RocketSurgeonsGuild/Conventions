using Rocket.Surgery.Conventions;
using Sample.DependencyOne;
using Sample.DependencyThree;

[assembly: Convention(typeof(Class3))]

namespace Sample.DependencyThree
{
    public class Class3 : IConvention
    {
        public Class1? Class1 { get; set; }
    }
}
