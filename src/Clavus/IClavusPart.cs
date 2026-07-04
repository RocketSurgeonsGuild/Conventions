namespace Clavus;

/// <summary>
///     A marker interface to indicate a type is a convention
/// </summary>
[PublicAPI]
public interface IClavusPart
{
    /// <summary>
    ///     The absolute priority of the convention.
    ///     If not provided the default value be 0.
    ///     Use a negative number to push conventions closer to the front.
    ///     Use a positive number to push conventions closer to the back.
    ///     Conventions will still be ordered based on dependencies, so
    ///     it is possible for a given convention to move front of a
    ///     convention using int.MinValue.
    /// </summary>
    int Priority => 0;

    /// <summary>
    ///    The value comparer for this category
    /// </summary>
    internal static IEqualityComparer<IClavusPart> ValueComparer { get; } = new ValueEqualityComparer();

    private sealed class ValueEqualityComparer : IEqualityComparer<IClavusPart>
    {
        public bool Equals(IClavusPart? x, IClavusPart? y) => ReferenceEquals(x, y) || ( x is { } && y is { } && x.GetType() == y.GetType() );

        public int GetHashCode(IClavusPart obj) => obj.GetType().GetHashCode();
    }

}
