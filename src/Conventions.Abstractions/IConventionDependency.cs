namespace Rocket.Surgery.Conventions;

/// <summary>
///     A dependency for a given convention
/// </summary>
public interface IConventionDependency
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
