using System.Collections.Immutable;
using System.Reflection;
using Clavus.Infrastructure;

namespace Clavus;

/// <summary>
///     The base context marker interface to define this as a context
/// </summary>
[PublicAPI]
public interface IClavusContext
{
    /// <summary>
    ///     The assembly that is executing the conventions
    /// </summary>
    // ReSharper disable once NullableWarningSuppressionIsUsed
    Assembly Assembly => this.Get<Assembly>("ExecutingAssembly") ?? Assembly.GetEntryAssembly()!;

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    ImmutableHashSet<ClavusCategory> Categories { get; }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    ImmutableHashSet<IClavusPart> Parts { get; }

    /// <summary>
    ///     The underlying host type
    /// </summary>
    HostType HostType { get; }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    IServiceProviderDictionary Properties { get; }

    internal ConventionExceptionPolicyDelegate ExceptionPolicy => this.Require<ConventionExceptionPolicyDelegate>();
}
