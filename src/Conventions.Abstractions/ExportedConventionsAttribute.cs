namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that has a list of exported conventions in an assembly (those that were contained in a [Convention] attribute)
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
///     The type to be used with the convention type
/// </remarks>
/// <param name="exportedConventions">The exported conventions.</param>
/// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
[PublicAPI]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ExportedConventionsAttribute(params Type[] exportedConventions) : Attribute
{

    /// <summary>
    ///     The convention types
    /// </summary>
    public Type[] ExportedConventions { get; } = exportedConventions;
}
