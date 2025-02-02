using System.Collections.Immutable;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     The base context marker interface to define this as a context
/// </summary>
[PublicAPI]
public interface IConventionContext
{
    /// <summary>
    ///     The assembly that is executing the conventions
    /// </summary>
    // ReSharper disable once NullableWarningSuppressionIsUsed
    public Assembly Assembly => this.Get<Assembly>("ExecutingAssembly") ?? Assembly.GetEntryAssembly()!;

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    ImmutableHashSet<ConventionCategory> Categories { get; }

    /// <summary>
    ///     The underlying configuration
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    IConventionProvider Conventions { get; }

    /// <summary>
    ///     The underlying host type
    /// </summary>
    HostType HostType { get; }

    /// <summary>
    ///     A logger that is configured to work with each convention item
    /// </summary>
    /// <value>The logger.</value>
    ILogger Logger { get; }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    IServiceProviderDictionary Properties { get; }

    internal ConventionExceptionPolicyDelegate ExceptionPolicy => this.Require<ConventionExceptionPolicyDelegate>();
}
