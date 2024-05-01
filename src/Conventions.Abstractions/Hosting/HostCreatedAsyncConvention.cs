namespace Rocket.Surgery.Conventions.Hosting;

/// <summary>
///     Delegate HostCreatedAsyncConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The host.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask HostCreatedAsyncConvention<in THost>(IConventionContext context, THost builder, CancellationToken cancellationToken);
