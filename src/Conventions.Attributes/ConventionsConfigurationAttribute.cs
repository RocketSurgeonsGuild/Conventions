namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base class to be used for both imports and exports for configuration
/// </summary>
public abstract class ConventionsConfigurationAttribute : Attribute
{
    /// <summary>
    ///     The desired namespace for the emitted classes to go
    /// </summary>
    public string Namespace { get; set; } = null!;

    /// <summary>
    ///     The desired class name for the emitted class.
    /// </summary>
    /// <remarks>
    ///     Default Imports or Exports
    /// </remarks>
    public string ClassName { get; set; } = null!;
}
