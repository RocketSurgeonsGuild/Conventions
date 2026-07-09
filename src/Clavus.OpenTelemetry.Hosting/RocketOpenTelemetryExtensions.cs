using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// ReSharper disable once CheckNamespace

namespace Clavus.OpenTelemetry;

[ClavusExport]
internal class OpenTelemetryPart : IServiceAsyncPart, IOpenTelemetryAsyncPart
{
    public ValueTask Register(IClavusContext context, IServiceCollection services, CancellationToken cancellationToken = default)
    {
        var otelBuilder = services.AddOpenTelemetry();
        return otelBuilder.ApplyOpenTelemetry(context, cancellationToken);
    }

    public ValueTask Register(IClavusContext context, IOpenTelemetryBuilder builder, CancellationToken cancellationToken = default)
    {
        builder.ConfigureResource(rb => rb.ApplyResourceBuilder(context, cancellationToken));
        builder.WithTracing(tp => tp.ApplyTracerProvider(context, cancellationToken));
        builder.WithMetrics(mp => mp.ApplyMeterProvider(context, cancellationToken));
        builder.WithLogging(lp => lp.ApplyLoggerProvider(context, cancellationToken));
        return ValueTask.CompletedTask;
    }
}
