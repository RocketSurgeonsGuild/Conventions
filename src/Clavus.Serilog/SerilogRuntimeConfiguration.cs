namespace Clavus.Serilog;

/// <summary>
///     Hand-authored stand-in for the generator-emitted configuration class that
///     <c>openspec/changes/clavus-managed-configuration</c> will eventually produce for
///     <c>appsettings.json</c>'s <c>Serilog</c> section (see design.md, Decision 4). Dogfoods the
///     runtime half of the pipeline (task 8.3) ahead of the generator landing.
/// </summary>
/// <remarks>
///     Once the codegen stage (tasks 3-4) ships, this class is expected to be deleted and replaced
///     by the generated equivalent — its shape (a settable-property sealed class matching the JSON
///     shape) is deliberately what the generator is documented to produce, so migrating on is a
///     drop-in replacement rather than a redesign.
/// </remarks>
public sealed class SerilogRuntimeConfiguration
{
    /// <summary>
    ///     The minimum Serilog level to emit, e.g. <c>"Information"</c>, <c>"Debug"</c>, <c>"Warning"</c>.
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    ///     Whether the console sink should be enabled.
    /// </summary>
    public bool WriteToConsole { get; set; } = true;
}
