using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: AssemblyProvider(typeof(AP))]

namespace Rocket.Surgery.Conventions;

/// <summary>
/// Attribute used to define the assembly provider for a given assembly
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyProviderAttribute(Type type) : Attribute
{
    // ReSharper disable once NullableWarningSuppressionIsUsed
    private Lazy<IAssemblyProvider> _assemblyProvider = new (() => (IAssemblyProvider)Activator.CreateInstance(type)!);

    /// <summary>
    /// The assembly provider
    /// </summary>
    public IAssemblyProvider AssemblyProvider => _assemblyProvider.Value;
}

file class AP : IAssemblyProvider {
    public IEnumerable<Assembly> GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, int lineNumber = 0, string filePath = "", string argumentExpression = "") => throw new NotImplementedException();

    public IEnumerable<Type> GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, int lineNumber = 0, string filePath = "", string argumentExpression = "") => throw new NotImplementedException();
}

/// <summary>
/// Assembly provider extensions
/// </summary>
public static class AssemblyProviderExtensions
{
    /// <summary>
    /// Get the assembly provider for the given assembly
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IAssemblyProvider GetAssemblyProvider(this Assembly? assembly) => assembly?.GetCustomAttribute<AssemblyProviderAttribute>()?.AssemblyProvider ?? throw new InvalidOperationException("No AssemblyProviderAttribute found on the assembly");
}

/// <summary>
///     A provider that gets a list of assemblies for a given context
/// </summary>
[PublicAPI]
public interface IAssemblyProvider
{
    /// <summary>
    ///     Method used to ensure the argument expression is hashed correctly each time.
    /// </summary>
    /// <param name="argumentExpression"></param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetArgumentExpressionHash(string argumentExpression)
    {
        var expression = argumentExpression.Replace("\r", "");
        expression = string.Join("", expression.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()));
        return Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(expression)));
    }

    /// <summary>
    /// The current assembly type provider
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static IAssemblyProvider Current => Assembly.GetEntryAssembly().GetAssemblyProvider();

    /// <summary>
    ///     Get the full list of assemblies
    /// </summary>
    /// <returns>IEnumerable{Assembly}.</returns>
    IEnumerable<Assembly> GetAssemblies(
        Action<IAssemblyProviderAssemblySelector> action,
        [CallerLineNumber]
        int lineNumber = 0,
        [CallerFilePath]
        string filePath = "",
        [CallerArgumentExpression(nameof(action))]
        string argumentExpression = ""
    );

    /// <summary>
    ///     Get the full list of types using the given selector
    /// </summary>
    /// <returns>IEnumerable{Type}.</returns>
    IEnumerable<Type> GetTypes(
        Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector,
        [CallerLineNumber]
        int lineNumber = 0,
        [CallerFilePath]
        string filePath = "",
        [CallerArgumentExpression(nameof(selector))]
        string argumentExpression = ""
    );
}
