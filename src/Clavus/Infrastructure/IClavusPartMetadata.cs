namespace Clavus.Infrastructure;

/// <summary>
///     Declares a convention result with it's dependencies pre-computed
/// </summary>
public interface IClavusPartMetadata
{
    /// <summary>
    ///     The convention
    /// </summary>
    IClavusPart Convention { get; }

    /// <summary>
    ///     The dependencies
    /// </summary>
    IEnumerable<IClavusDependency> Dependencies { get; }

    /// <summary>
    ///     The host type of the convention
    /// </summary>
    HostType HostType { get; }

    /// <summary>
    ///     The category of the convention
    /// </summary>
    ClavusCategory Category { get; }
}
