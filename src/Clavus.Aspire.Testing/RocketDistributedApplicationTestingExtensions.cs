using System.ComponentModel;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Configuration;

#pragma warning disable CA1031, CA2000, CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Clavus.Aspire.Testing;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RocketDistributedApplicationTestingExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<DistributedApplication> Configure(
        IDistributedApplicationTestingBuilder builder,
        ClavusContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextBuilder);

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing(builder.Configuration)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.Configuration.GetType(), builder.Configuration)
           .AddIfMissing(builder.Environment)
           .AddIfMissing(nameof(builder.Environment.ApplicationName), builder.Environment.ApplicationName)
           .AddIfMissing(nameof(builder.Environment.ContentRootPath), builder.Environment.ContentRootPath)
           .AddIfMissing(nameof(builder.Environment.EnvironmentName), builder.Environment.EnvironmentName)
           .AddIfMissing(builder.Environment.GetType(), builder.Environment);

        var context = await ClavusContext.FromAsync(contextBuilder, cancellationToken).ConfigureAwait(false);
        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), }
        );

        await builder.ApplyPartsAsync(context, cancellationToken).ConfigureAwait(false);
        var host = await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
        //        await context.ApplyHostCreatedPartsAsync(host, cancellationToken).ConfigureAwait(false);
        return host;
    }
}
