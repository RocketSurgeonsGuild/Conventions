using Microsoft.Extensions.DependencyModel;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
public class DependencyContextConventionFactory(DependencyContext dependencyContext) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder)
    {
        return new DependencyContextAssemblyProvider(dependencyContext);
    }
}
