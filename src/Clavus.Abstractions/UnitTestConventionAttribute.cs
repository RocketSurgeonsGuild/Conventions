namespace Rocket.Surgery.Clavus;

/// <summary>
///     Defines this convention as one that only runs during a unit test run
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public sealed class UnitTestPartAttribute : Attribute, IHostBasedPart
{
    HostType IHostBasedPart.HostType => HostType.UnitTest;
}
