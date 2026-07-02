using Aspire.Hosting.Testing;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire.Testing;

/// <summary>
///     Delegate DistributedApplicationTestingAsyncConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask DistributedApplicationTestingAsyncConvention(IClavusContext context, IDistributedApplicationTestingBuilder builder, CancellationToken cancellationToken);
