namespace Rocket.Surgery.Clavus;

/// <summary>
///     A dependency for a given convention
/// </summary>
public interface IClavusDependency
{
    /// <summary>
    ///     The type
    /// </summary>
    Type Type { get; }

    /// <summary>
    ///     The direction
    /// </summary>
    DependencyDirection Direction { get; }
}
