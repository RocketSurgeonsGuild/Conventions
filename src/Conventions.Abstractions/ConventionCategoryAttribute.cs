namespace Rocket.Surgery.Conventions;

/// <summary>
///     Defines the category of a given convention
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class ConventionCategoryAttribute(string category) : Attribute
{
    /// <summary>
    ///     The category of a given convention
    /// </summary>
    public ConventionCategory Category { get; } = category;
}
