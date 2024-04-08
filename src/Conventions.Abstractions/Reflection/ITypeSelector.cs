namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     The Compiled Implementation Type Selector
/// </summary>
[PublicAPI]
public interface ITypeSelector : ITypeProviderAssemblySelector
{
    /// <summary>
    ///     Lists all of the classes in a given assembly
    /// </summary>
    [RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
    IEnumerable<Type> GetTypes();

    /// <summary>
    ///     Lists all of the public classes in a given assembly
    /// </summary>
    /// <param name="publicOnly">Specifies whether too add public types only.</param>
    [RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
    IEnumerable<Type> GetTypes(bool publicOnly);

    /// <summary>
    ///     Adds all public, non-abstract classes from the selected assemblies that
    ///     matches the requirements specified in the <paramref name="action" />
    ///     to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <param name="action">The filtering action.</param>
    [RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
    IEnumerable<Type> GetTypes(Action<ITypeFilter> action);

    /// <summary>
    ///     Adds all non-abstract classes from the selected assemblies that
    ///     matches the requirements specified in the <paramref name="action" />
    ///     to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <param name="action">The filtering action.</param>
    /// <param name="publicOnly">Specifies whether too add public types only.</param>
    [RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
    IEnumerable<Type> GetTypes(bool publicOnly, Action<ITypeFilter> action);
}
