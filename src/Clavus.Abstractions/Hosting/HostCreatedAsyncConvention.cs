namespace Rocket.Surgery.Clavus.Hosting;

/// <summary>
///     Delegate HostCreatedAsyncPart
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The host.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask HostCreatedAsyncPart<in THost>(IClavusContext context, THost builder, CancellationToken cancellationToken);
