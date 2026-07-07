namespace Clavus.Configuration.Runtime.Tests;

/// <summary>
///     Stand-in for a generator-emitted configuration class (see
///     <c>openspec/changes/clavus-managed-configuration/design.md</c>, Decision 4). The real generated
///     class will look exactly like this: a settable-property sealed class bound from a configuration
///     section, suitable for re-binding on reload.
/// </summary>
public sealed class SampleOptions
{
    public string Name { get; set; } = "";

    public int Count { get; set; }
}
