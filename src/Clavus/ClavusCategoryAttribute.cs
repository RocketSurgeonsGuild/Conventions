namespace Clavus;

/// <summary>
///     Defines the category of a given convention
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class ClavusCategoryAttribute(string category) : Attribute
{
    /// <summary>
    ///     The category of a given convention
    /// </summary>
    public ClavusCategory Category { get; } = category;
}
