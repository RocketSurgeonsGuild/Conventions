using Clavus.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Serilog;

/// <summary>
///     Hand-authored stand-in for the generator-emitted <c>IConfigurationPart</c> that
///     <c>openspec/changes/clavus-managed-configuration</c> will eventually emit for this library's
///     <c>appsettings.json</c> (see design.md, Decision 4). Registers
///     <see cref="SerilogRuntimeConfiguration" /> through the same hook
///     (<see cref="ClavusConfigurationOptionsExtensions.AddClavusConfigurationOptions{TOptions}" />)
///     the generator is expected to call, so this dogfood composes without changes once the
///     generator lands and this handwritten registration is retired.
/// </summary>
[ClavusExport]
public sealed class SerilogConfigurationPart : IServicePart
{
    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Dogfood stand-in for generator-emitted binding; see AddClavusConfigurationOptions.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Dogfood stand-in for generator-emitted binding; see AddClavusConfigurationOptions.")]
    public void Register(IClavusContext context, IServiceCollection services) =>
        services.AddClavusConfigurationOptions<SerilogRuntimeConfiguration>(context.Configuration, "Serilog");
}
