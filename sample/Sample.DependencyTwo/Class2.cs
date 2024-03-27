using Rocket.Surgery.Conventions;
using Sample.DependencyTwo;

[assembly: ExportConventions(Namespace = null, ClassName = "Dep2Exports")]
[assembly: Convention<Class2>]

namespace Sample.DependencyTwo;

public class Class2 : IConvention;