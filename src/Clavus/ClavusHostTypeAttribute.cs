namespace Clavus;

/// <summary>
///     Defines the category of a given convention
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class ClavusHostTypeAttribute(HostType hostType) : Attribute
{
    /// <summary>
    ///     The host type of a given convention
    /// </summary>
    public HostType HostType { get; } = hostType;
}
