using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Extensions;

#pragma warning disable CA1031
#pragma warning disable CA2000
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Aspire.Hosting.Testing;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RocketDistributedApplicationTestingExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<DistributedApplication> Configure(
        IDistributedApplicationTestingBuilder builder,
        ConventionContextBuilder contextBuilder,
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
           .AddIfMissing(builder.Environment.GetType(), builder.Environment);

        var context = await ConventionContext.FromAsync(contextBuilder, cancellationToken).ConfigureAwait(false);
        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), }
        );

        await builder.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);
        var host = await builder.BuildAsync(cancellationToken).ConfigureAwait(false);
        await context.ApplyHostCreatedConventionsAsync(host, cancellationToken).ConfigureAwait(false);
        return host;
    }
}
