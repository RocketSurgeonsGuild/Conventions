using System.Diagnostics;

namespace Rocket.Surgery.Conventions;

/// <summary>
/// The category of a given convention
/// </summary>
/// <remarks>
/// This is used to load limited sets of conventions based on categories provided.
/// </remarks>
[DebuggerDisplay("{_value}")]
public sealed class ConventionCategory(string name)
{
    private bool Equals(ConventionCategory other)
    {
        return _value == other._value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConventionCategory)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    /// <summary>
    /// This convention is loaded for any application that might be starting
    /// </summary>
    /// <remarks>Application is the default category for a convention</remarks>
    public const string Application = nameof(Application);

    /// <summary>
    /// This convention is to load for any infrastructure bits (serializer, logging, etc)
    /// </summary>
    public const string Infrastructure = nameof(Infrastructure);

    private readonly string _value = name;

    /// <summary>
    ///   Implicitly convert to a string
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static implicit operator string(ConventionCategory category) => category._value;

    /// <summary>
    ///  Implicitly convert from a string
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static implicit operator ConventionCategory(string category) => new(category);

    /// <inheritdoc />
    public override string ToString() => _value;
}
