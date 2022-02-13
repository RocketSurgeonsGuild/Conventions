namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base class to be used for both imports and exports for configuration
/// </summary>
public abstract class ConventionsConfigurationAttribute : Attribute
{
    /// <summary>
    ///     Create a new class (using <see cref="ClassName" />) with these conventions
    /// </summary>
    public bool Assembly { get; set; }

    /// <summary>
    ///     Should the default namespace has a the .Conventions postfix
    /// </summary>
    /// <returns></returns>
    public bool Postfix { get; set; } = true;

    /// <summary>
    ///     The desired namespace for the emitted classes to go
    /// </summary>
    public string? Namespace { get; set; } = null;

    /// <summary>
    ///     The desired class name for the emitted class.
    /// </summary>
    /// <remarks>
    ///     Default Imports or Exports
    /// </remarks>
    public string ClassName { get; set; } = null!;

    /// <summary>
    ///     The method name to use when attaching to a class
    /// </summary>
    public string MethodName { get; set; } = null!;
}
