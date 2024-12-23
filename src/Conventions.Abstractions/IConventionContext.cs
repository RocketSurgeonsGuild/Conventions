using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     The base context marker interface to define this as a context
/// </summary>
[PublicAPI]
public interface IConventionContext
{
    /// <summary>
    ///     The underlying host type
    /// </summary>
    HostType HostType { get; }

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    ImmutableHashSet<ConventionCategory> Categories { get; }

    /// <summary>
    ///     Allows a context to hold additional information for conventions to consume such as configuration objects
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>System.Object.</returns>
    object this[object item] { get; set; }

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
    ///     Gets the type provider.
    /// </summary>
    /// <value>The type provider.</value>
    ICompiledTypeProvider TypeProvider { get; }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    IConventionProvider Conventions { get; }

    /// <summary>
    ///     The underlying configuration
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    ///     Returns the source builder for this context
    /// </summary>
    /// <returns></returns>
    ConventionContextBuilder ToBuilder();
}
