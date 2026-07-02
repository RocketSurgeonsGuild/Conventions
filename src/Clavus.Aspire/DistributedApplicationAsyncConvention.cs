using Aspire.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire;

/// <summary>
///     Delegate DistributedApplicationAsyncConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask DistributedApplicationAsyncConvention(IClavusContext context, IDistributedApplicationBuilder builder, CancellationToken cancellationToken);
