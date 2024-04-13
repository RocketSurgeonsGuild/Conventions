using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

/// <inheritdoc />
public class DependencyContextConventionFactory(DependencyContext dependencyContext) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new DependencyContextAssemblyProvider(dependencyContext);
}