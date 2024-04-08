namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base class to be used for both imports and exports for configuration
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
public abstract class ConventionsConfigurationAttribute : Attribute
{
    /// <summary>
    ///     Create a new class (using <see cref="ClassName" />) with these conventions
    /// </summary>
    public bool Assembly { get; set; }

    /// <summary>
    ///     The desired namespace for the emitted classes to go
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    ///     The desired class name for the emitted class.
    /// </summary>
    /// <remarks>
    ///     Default Imports or Exports
    /// </remarks>
    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    public string ClassName { get; set; } = null!;

    /// <summary>
    ///     The method name to use when attaching to a class
    /// </summary>
    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    public string MethodName { get; set; } = null!;
}
