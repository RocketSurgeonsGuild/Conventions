namespace Rocket.Surgery.Clavus;

/// <summary>
///     Ensures the convention runs after the given <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
///     Default constructor
/// </remarks>
/// <param name="direction"></param>
/// <param name="type"></param>
internal readonly struct ClavusDependency(DependencyDirection direction, Type type) : IEquatable<ClavusDependency>, IClavusDependency
{

    /// <summary>
    ///     The <see cref="IClavusPart" /> type to link to
    /// </summary>
    public Type Type { get; } = type;

    /// <summary>
    ///     The <see cref="DependencyDirection" /> direction of this relationship
    /// </summary>
    public DependencyDirection Direction { get; } = direction;

    /// <summary>
    ///     Equals
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ClavusDependency other) => Type == other.Type && Direction == other.Direction;

    /// <summary>
    ///     Compare equality
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is ClavusDependency other && Equals(other);

    /// <summary>
    ///     Get hashcode
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ( Type.GetHashCode() * 397 ) ^ (int)Direction;
        }
    }

    /// <summary>
    ///     Equals
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(ClavusDependency left, ClavusDependency right) => left.Equals(right);

    /// <summary>
    ///     Not Equals
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(ClavusDependency left, ClavusDependency right) => !left.Equals(right);
}
