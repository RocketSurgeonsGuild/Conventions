using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
public class AssemblyConventionFactory(IEnumerable<Assembly> assemblies) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new DefaultAssemblyProvider(assemblies);
}
