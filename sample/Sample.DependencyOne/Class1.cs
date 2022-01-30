using Rocket.Surgery.Conventions;
using Sample.DependencyOne;

[assembly: ExportConventions(Namespace = "Dep1", ClassName = "Dep1Exports")]
[assembly: Convention(typeof(Class1))]

namespace Sample.DependencyOne;

public class Class1 : IConvention
{
}
