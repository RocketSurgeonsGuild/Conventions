namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that has a list of exported conventions in an assembly (those that were contained in a [Convention] attribute)
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ExportedConventionsAttribute : Attribute
{
    /// <summary>
    ///     The convention types
    /// </summary>
    public Type[] ExportedConventions { get; }

    /// <summary>
    ///     The type to be used with the convention type
    /// </summary>
    /// <param name="exportedConventions">The exported conventions.</param>
    /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
    public ExportedConventionsAttribute(params Type[] exportedConventions)
    {
        ExportedConventions = exportedConventions;
    }
}
