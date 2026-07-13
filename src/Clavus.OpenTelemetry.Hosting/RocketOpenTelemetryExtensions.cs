using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

// ReSharper disable once CheckNamespace

namespace Clavus.OpenTelemetry;

[ClavusExport]
internal class OpenTelemetryPart : IServiceAsyncPart, IOpenTelemetryPart
{
    public ValueTask Register(IClavusContext context, IServiceCollection services, CancellationToken cancellationToken = default)
    {
        var otelBuilder = services.AddOpenTelemetry();
        return otelBuilder.ApplyOpenTelemetry(context, cancellationToken);
    }

    public void Register(IClavusContext context, IOpenTelemetryBuilder builder)
    {
        builder.ConfigureResource(rb => rb.ApplyResourceBuilder(context));
        builder.WithTracing(tp => tp.ApplyTracerProvider(context));
        builder.WithMetrics(mp => mp.ApplyMeterProvider(context));
        builder.WithLogging(lp => lp.ApplyLoggerProvider(context));
    }
}
