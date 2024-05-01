using Aspire.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting;

/// <summary>
///     Delegate DistributedApplicationAsyncConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask DistributedApplicationAsyncConvention(IConventionContext context, IDistributedApplicationBuilder builder, CancellationToken cancellationToken);
