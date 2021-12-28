using System.Diagnostics;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that will add a static method onto the given type that returns all the conventions of all the referenced assemblies
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
[Conditional("CodeGeneration")]
public sealed class ImportConventionsAttribute : Attribute
{
}
