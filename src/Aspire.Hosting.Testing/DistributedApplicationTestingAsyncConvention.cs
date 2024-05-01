using Aspire.Hosting.Testing;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

/// <summary>
///     Delegate DistributedApplicationTestingAsyncConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask DistributedApplicationTestingAsyncConvention(IConventionContext context, IDistributedApplicationTestingBuilder builder, CancellationToken cancellationToken);
