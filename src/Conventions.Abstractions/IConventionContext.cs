using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     The base context marker interface to define this as a context
/// </summary>
public interface IConventionContext
{
    /// <summary>
    ///     Allows a context to hold additional information for conventions to consume such as configuration objects
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>System.Object.</returns>
    object? this[object item] { get; set; }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    IServiceProviderDictionary Properties { get; }

    /// <summary>
    ///     A logger that is configured to work with each convention item
    /// </summary>
    /// <value>The logger.</value>
    ILogger Logger { get; }

    /// <summary>
    ///     Gets the assembly provider.
    /// </summary>
    /// <value>The assembly provider.</value>
    IAssemblyProvider AssemblyProvider { get; }

    /// <summary>
    ///     Gets the assembly candidate finder.
    /// </summary>
    /// <value>The assembly candidate finder.</value>
    IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    IConventionProvider Conventions { get; }
}
