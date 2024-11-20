using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
public class DependencyContextConventionFactory(DependencyContext dependencyContext) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override ICompiledTypeProvider CreateTypeProvider(ConventionContextBuilder builder)
    {
        return new DependencyContextAssemblyProvider(dependencyContext);
    }
}
