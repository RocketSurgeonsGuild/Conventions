using Aspire.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting;

/// <summary>
///     Delegate DistributedApplicationConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
[PublicAPI]
public delegate void DistributedApplicationConvention(IConventionContext context, IDistributedApplicationBuilder builder);
