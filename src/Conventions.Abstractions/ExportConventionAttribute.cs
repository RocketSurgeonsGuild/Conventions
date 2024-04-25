using System.Diagnostics;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     An alternative to the [assembly: Convention] attribute, to export a convention from the class itself.
/// </summary>
/// <remarks>
///     Only works with source generators enabled.
/// </remarks>
/// <seealso cref="Attribute" />
[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
[Conditional("CodeGeneration")]
public sealed class ExportConventionAttribute : Attribute;