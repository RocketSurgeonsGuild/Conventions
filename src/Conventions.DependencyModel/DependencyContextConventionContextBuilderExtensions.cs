using Microsoft.Extensions.DependencyModel;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extensions to support dependency context
/// </summary>
public static class DependencyContextConventionContextBuilderExtensions
{
    /// <summary>
    ///     Use the given dependency context for resolving assemblies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="dependencyContext"></param>
    /// <returns></returns>
    public static ConventionContextBuilder UseDependencyContext(this ConventionContextBuilder builder, DependencyContext dependencyContext)
    {
        return builder.UseConventionFactory(new DependencyContextConventionFactory(dependencyContext));
    }
}
