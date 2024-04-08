namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     The compiled assembly selector
/// </summary>
[PublicAPI]
public interface IAssemblyProviderAssemblySelector
{
    /// <summary>
    ///     Will scan for types from this assembly at compile time.
    /// </summary>
    IAssemblyProviderAssemblySelector FromAssembly();

    /// <summary>
    ///     Will scan for types from all metadata assembly at compile time.
    /// </summary>
    IAssemblyProviderAssemblySelector FromAssemblies();

    /// <summary>
    ///     With load any assembly dependencies of the given type.
    /// </summary>
    /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
    IAssemblyProviderAssemblySelector FromAssemblyDependenciesOf<T>();

    /// <summary>
    ///     With load any assembly dependencies of the given type.
    /// </summary>
    IAssemblyProviderAssemblySelector FromAssemblyDependenciesOf(Type type);

    /// <summary>
    ///     Will add the assembly of the given type.
    /// </summary>
    /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
    IAssemblyProviderAssemblySelector FromAssemblyOf<T>();

    /// <summary>
    ///     Will add the assembly of the given type.
    /// </summary>
    IAssemblyProviderAssemblySelector FromAssemblyOf(Type type);
}
