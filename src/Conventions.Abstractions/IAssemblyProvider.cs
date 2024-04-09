using System.Reflection;
using System.Runtime.CompilerServices;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     A provider that gets a list of assemblies for a given context
/// </summary>
[PublicAPI]
public interface IAssemblyProvider
{
    /// <summary>
    ///     Get the full list of assemblies
    /// </summary>
    /// <returns>IEnumerable{Assembly}.</returns>
    IEnumerable<Assembly> GetAssemblies(
        Action<IAssemblyProviderAssemblySelector> action,
        [CallerFilePath]
        string filePath = "",
        [CallerMemberName]
        string memberName = "",
        [CallerLineNumber]
        int lineNumber = 0
    );

    /// <summary>
    ///     Get the full list of types using the given selector
    /// </summary>
    /// <returns>IEnumerable{Type}.</returns>
    IEnumerable<Type> GetTypes(
        Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector,
        [CallerFilePath]
        string filePath = "",
        [CallerMemberName]
        string memberName = "",
        [CallerLineNumber]
        int lineNumber = 0
    );
}
