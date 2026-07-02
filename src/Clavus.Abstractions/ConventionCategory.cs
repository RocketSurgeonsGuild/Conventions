using System.Diagnostics;

namespace Rocket.Surgery.Clavus;

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
public sealed class ClavusCategory(string name)
{
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is { } && ( ReferenceEquals(this, obj) || ( obj.GetType() == GetType() && Equals((ClavusCategory)obj) ) );

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => _value;

    /// <summary>
    ///     Implicitly convert to a string
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static implicit operator string(ClavusCategory category) => category._value;

    /// <summary>
    ///     Implicitly convert from a string
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static implicit operator ClavusCategory(string category) => new(category);

    public static IEqualityComparer<ClavusCategory> ValueComparer { get; } = new ValueEqualityComparer();

    /// <summary>
    ///     This convention is loaded for any application that might be starting
    /// </summary>
    /// <remarks>Application is the default category for a convention</remarks>
    public const string Application = nameof(Application);

    /// <summary>
    ///     This convention is to load for any infrastructure bits (serializer, logging, etc)
    /// </summary>
    public const string Core = nameof(Core);

    private sealed class ValueEqualityComparer : IEqualityComparer<ClavusCategory>
    {
        public bool Equals(ClavusCategory? x, ClavusCategory? y) => ReferenceEquals(x, y) || ( x is { } && y is { } && x.GetType() == y.GetType() && x._value == y._value );

        public int GetHashCode(ClavusCategory obj) => obj._value.GetHashCode();
    }

    private bool Equals(ClavusCategory other) => _value == other._value;

    private readonly string _value = name;
}
