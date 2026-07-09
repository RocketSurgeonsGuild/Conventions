using OpenTelemetry.Trace;

namespace Clavus.OpenTelemetry.Tests;

public static class OpenTelemetryFixtures
{
    /// <summary>
    ///     Activity source name used by the auto-discovered <see cref="AutoDiscoveredTracerPart" /> (task 4.7).
    /// </summary>
    public const string AutoDiscoveredSourceName = "Clavus.OpenTelemetry.Tests.AutoDiscovered";

    /// <summary>
    ///     A tracer part that is discovered purely via <see cref="ClavusExportAttribute" /> assembly scanning,
    ///     with no manual <c>ConfigureTracerProvider</c> call (task 4.7).
    /// </summary>
    [ClavusExport]
    public sealed class AutoDiscoveredTracerPart : ITracerProviderPart
    {
        public void Register(IClavusContext context, TracerProviderBuilder builder) => builder.AddSource(AutoDiscoveredSourceName);
    }
}
