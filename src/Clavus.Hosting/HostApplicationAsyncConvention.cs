using Microsoft.Extensions.Hosting;

namespace Clavus.Hosting;

/// <summary>
///     Delegate HostApplicationPart
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask HostApplicationAsyncPart<in TBuilder>(IClavusContext context, TBuilder builder, CancellationToken cancellationToken)
    where TBuilder : IHostApplicationBuilder;
