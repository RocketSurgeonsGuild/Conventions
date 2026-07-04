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
    IReadOnlyCollection<IClavusDependency> Dependencies { get; }

    /// <summary>
    ///     The host type of the convention
    /// </summary>
    HostType HostType { get; }

    /// <summary>
    ///     The category of the convention
    /// </summary>
    ClavusCategory Category { get; }

    /// <summary>
    ///    The value comparer for this category
    /// </summary>
    internal static IEqualityComparer<IClavusPartMetadata> ValueComparer { get; } = new ValueEqualityComparer();

    private sealed class ValueEqualityComparer : IEqualityComparer<IClavusPartMetadata>
    {
        public bool Equals(IClavusPartMetadata? x, IClavusPartMetadata? y) => ReferenceEquals(x?.Convention, y?.Convention) || ( x is { } && y is { } && x.Convention.GetType() == y.Convention.GetType() );

        public int GetHashCode(IClavusPartMetadata obj) => obj.Convention.GetType().GetHashCode();
    }
}
