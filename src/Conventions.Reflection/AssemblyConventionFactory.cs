using System.Reflection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
public class AssemblyConventionFactory(IEnumerable<Assembly> assemblies) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override ICompiledTypeProvider CreateTypeProvider(ConventionContextBuilder builder)
    {
        return new DefaultAssemblyProvider(assemblies);
    }
}
