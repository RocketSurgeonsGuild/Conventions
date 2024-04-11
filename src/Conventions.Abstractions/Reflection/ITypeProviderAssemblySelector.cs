namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     The compiled assembly selector
/// </summary>
[PublicAPI]
public interface ITypeProviderAssemblySelector
{
    /// <summary>
    ///     Will scan for types from this assembly at compile time.
    /// </summary>
    ITypeSelector FromAssembly();

    /// <summary>
    ///     Will scan for types from all metadata assembly at compile time.
    /// </summary>
    ITypeSelector FromAssemblies();

    /// <summary>
    ///     Will load and scan from given types assembly
    /// </summary>
    ITypeSelector FromAssemblyDependenciesOf<T>();

    /// <summary>
    ///     Will load and scan from given types assembly
    /// </summary>
    ITypeSelector FromAssemblyDependenciesOf(Type type);

    /// <summary>
    ///     Will scan for types from the assembly of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
    ITypeSelector FromAssemblyOf<T>();

    /// <summary>
    ///     Will scan for types from the assembly of type.
    /// </summary>
    ITypeSelector FromAssemblyOf(Type type);

    /// <summary>
    ///     Will not scan for types from the assembly of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
    ITypeSelector NotFromAssemblyOf<T>();

    /// <summary>
    ///     Will not scan for types from the assembly of type.
    /// </summary>
    ITypeSelector NotFromAssemblyOf(Type type);

    /// <summary>
    /// Include system assemblies
    /// </summary>
    /// <returns></returns>
    ITypeSelector IncludeSystemAssemblies();
}
