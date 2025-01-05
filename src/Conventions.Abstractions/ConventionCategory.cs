using System.Diagnostics;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     A policy delegate that can be used to determine if a given exception should be ignored or not
/// </summary>
public delegate bool ConventionExceptionPolicyDelegate(Exception exception);

public static class ConventionExceptionPolicy
{
    public static ConventionExceptionPolicyDelegate IgnoreNotSupported { get; } = exception => exception is NotSupportedException;
}

/// <summary>
///     The category of a given convention
/// </summary>
/// <remarks>
///     This is used to load limited sets of conventions based on categories provided.
/// </remarks>
[DebuggerDisplay("{_value}")]
public sealed class ConventionCategory(string name)
{
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is { } && ( ReferenceEquals(this, obj) || ( obj.GetType() == GetType() && Equals((ConventionCategory)obj) ) );

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => _value;

    /// <summary>
    ///     Implicitly convert to a string
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static implicit operator string(ConventionCategory category) => category._value;

    /// <summary>
    ///     Implicitly convert from a string
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static implicit operator ConventionCategory(string category) => new(category);

    public static IEqualityComparer<ConventionCategory> ValueComparer { get; } = new ValueEqualityComparer();

    /// <summary>
    ///     This convention is loaded for any application that might be starting
    /// </summary>
    /// <remarks>Application is the default category for a convention</remarks>
    public const string Application = nameof(Application);

    /// <summary>
    ///     This convention is to load for any infrastructure bits (serializer, logging, etc)
    /// </summary>
    public const string Core = nameof(Core);

    private sealed class ValueEqualityComparer : IEqualityComparer<ConventionCategory>
    {
        public bool Equals(ConventionCategory? x, ConventionCategory? y) => ReferenceEquals(x, y) || ( x is { } && y is { } && x.GetType() == y.GetType() && x._value == y._value );

        public int GetHashCode(ConventionCategory obj) => obj._value.GetHashCode();
    }

    private bool Equals(ConventionCategory other) => _value == other._value;

    private readonly string _value = name;
}
