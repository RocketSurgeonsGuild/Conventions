using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;

namespace Clavus.Hosting;

[ClavusExport]
internal class OpenTelemetryPart : IServiceAsyncPart, IOpenTelemetryAsyncPart
{
    public ValueTask Register(IClavusContext context, IServiceCollection services, CancellationToken cancellationToken = default)
    {
        var builder = new Builder(services);
        return builder.ApplyOpenTelemetry(context, cancellationToken);
    }

    public ValueTask Register(IClavusContext context, IOpenTelemetryBuilder builder, CancellationToken cancellationToken = default)
    {
        builder.ConfigureResource(rb => rb.ApplyResourceBuilder(context, cancellationToken));
        builder.WithTracing(tp => tp.ApplyTracerProvider(context, cancellationToken));
        builder.WithMetrics(mp => mp.ApplyMeterProvider(context, cancellationToken));
        builder.WithLogging(lp => lp.ApplyLoggerProvider(context, cancellationToken));
        return ValueTask.CompletedTask;
    }

    private class Builder(IServiceCollection services) : IOpenTelemetryBuilder
    {
        public IServiceCollection Services { get; } = services;
    }
}
