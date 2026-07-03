using System.ComponentModel;

namespace Clavus.Infrastructure;

/// <summary>
///    Defines a type that is imported into the assembly
/// </summary>
/// <param name="type"></param>
[EditorBrowsable(EditorBrowsableState.Never)]
[AttributeUsage(AttributeTargets.Assembly)]
public class ImportsTypeAttribute(Type type) : Attribute
{
    /// <summary>
    ///   The type that is imported into the assembly
    /// </summary>
    public Type Type { get; } = type;
}
