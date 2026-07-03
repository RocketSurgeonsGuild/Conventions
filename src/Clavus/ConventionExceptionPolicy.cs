namespace Clavus;

/// <summary>
///     A policy that determines how to handle exceptions thrown by conventions
/// </summary>
public static class ConventionExceptionPolicy
{
    /// <summary>
    ///    A policy that ignores <see cref="NotSupportedException" /> exceptions
    /// </summary>
    public static ConventionExceptionPolicyDelegate IgnoreNotSupported { get; } = exception => exception is NotSupportedException;
}
