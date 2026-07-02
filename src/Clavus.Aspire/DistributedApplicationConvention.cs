using Aspire.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire;

/// <summary>
///     Delegate DistributedApplicationConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
[PublicAPI]
public delegate void DistributedApplicationConvention(IClavusContext context, IDistributedApplicationBuilder builder);
