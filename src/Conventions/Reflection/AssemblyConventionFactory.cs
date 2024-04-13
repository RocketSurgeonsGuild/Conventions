using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
public class AssemblyConventionFactory(IEnumerable<Assembly> assemblies) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new DefaultAssemblyProvider(assemblies);
}