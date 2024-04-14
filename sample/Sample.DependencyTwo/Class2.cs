using Rocket.Surgery.Conventions;

[assembly: ExportConventions(Namespace = null, ClassName = "Dep2Exports")]
namespace Sample.DependencyTwo;

public static class Nested
{
    [ExportConvention]
    public class Class2 : IConvention;
}
