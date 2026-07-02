namespace Rocket.Surgery.Clavus;

/// <summary>
///     Defines this convention as one that only runs during live usage to avoid unit tests
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public sealed class LivePartAttribute : Attribute, IHostBasedPart
{
    HostType IHostBasedPart.HostType => HostType.Live;
}
