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
    private static readonly ConditionalWeakTable<IDistributedApplicationTestingBuilder, ConventionContextBuilder> _weakTable = new();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ConventionContextBuilder GetExisting(IDistributedApplicationTestingBuilder builder)
    {
        var contextBuilder = _weakTable.TryGetValue(builder, out var ccb)
            ? ccb
            : new(new Dictionary<object, object>());
        ccb = ImportHelpers.CallerConventions(Assembly.GetCallingAssembly()) is { } impliedFactory
            ? contextBuilder.UseConventionFactory(impliedFactory)
            : contextBuilder;
        _weakTable.Add(builder, ccb);
        return ccb;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask<DistributedApplication> Configure(
        IDistributedApplicationTestingBuilder builder,
        ConventionContextBuilder contextBuilder,
        CancellationToken cancellationToken
    )
    {
        if (contextBuilder.Properties.ContainsKey("__configured__")) throw new NotSupportedException("Cannot configure conventions on the same builder twice");
        contextBuilder.Properties["__configured__"] = true;

        contextBuilder
           .AddIfMissing(HostType.Live)
           .AddIfMissing(builder)
           .AddIfMissing(builder.GetType(), builder)
           .AddIfMissing(builder.Configuration)
           .AddIfMissing<IConfiguration>(builder.Configuration)
           .AddIfMissing(builder.Configuration.GetType(), builder.Configuration)
           .AddIfMissing(builder.Environment)
           .AddIfMissing(builder.Environment.GetType(), builder.Environment);

        var context = await ConventionContext.FromAsync(contextBuilder, cancellationToken);
        await builder.ApplyConventionsAsync(context, cancellationToken);
        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), }
        );
        await builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> { ["RocketSurgeryConventions:HostType"] = context.GetHostType().ToString(), })
                     .ApplyConventionsAsync(context, builder.Configuration, cancellationToken);
        await builder.Services.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);

        var host = await builder.BuildAsync(cancellationToken);
        await context.ApplyHostCreatedConventionsAsync(host, cancellationToken);
        return host;
    }
}
