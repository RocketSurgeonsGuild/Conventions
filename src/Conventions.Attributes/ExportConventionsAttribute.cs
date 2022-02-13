namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that lets you configure how exported conventions will behave
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
public sealed class ExportConventionsAttribute : ConventionsConfigurationAttribute
{
}
