using System.Reflection;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Convention Context build extensions.
/// </summary>
[PublicAPI]
public static class ReflectionConventionContextBuilderExtensions
{
    /// <summary>
    ///     Use the given app domain for resolving assemblies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="appDomain"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
    public static ConventionContextBuilder UseAppDomain(this ConventionContextBuilder builder, AppDomain appDomain)
    {
        return builder.UseConventionFactory(new AppDomainConventionFactory(appDomain));
    }

    /// <summary>
    ///     Use the given set of assemblies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
    public static ConventionContextBuilder UseAssemblies(this ConventionContextBuilder builder, IEnumerable<Assembly> assemblies)
    {
        return builder.UseConventionFactory(new AssemblyConventionFactory(assemblies));
    }
}
