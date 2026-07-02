using Aspire.Hosting.Testing;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire.Testing;

/// <summary>
///     Delegate DistributedApplicationTestingConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
[PublicAPI]
public delegate void DistributedApplicationTestingConvention(IClavusContext context, IDistributedApplicationTestingBuilder builder);
