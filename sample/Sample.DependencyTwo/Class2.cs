using Rocket.Surgery.Conventions;
using Sample.DependencyTwo;

[assembly: ExportConventions(Namespace = "Dep2", ClassName = "Dep2Exports")]
[assembly: Convention(typeof(Class2))]

namespace Sample.DependencyTwo;

public class Class2 : IConvention
{
}
