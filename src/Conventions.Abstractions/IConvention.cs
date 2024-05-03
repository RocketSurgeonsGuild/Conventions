#pragma warning disable CA1040
namespace Rocket.Surgery.Conventions;

/// <summary>
///     A marker interface to indicate a type is a convention
/// </summary>
[PublicAPI]
public interface IConvention
{
    /// <summary>
    /// The absolute priority of the convention.
    ///
    /// If not provided the default value be 0.
    /// Use a negative number to push conventions closer to the front.
    /// Use a positive number to push conventions closer to the back.
    /// Conventions will still be ordered based on dependencies, so
    /// it is possible for a given convention to move front of a
    /// convention using int.MinValue.
    /// </summary>
    public int Priority => 0;
}

#pragma warning restore CA1040
