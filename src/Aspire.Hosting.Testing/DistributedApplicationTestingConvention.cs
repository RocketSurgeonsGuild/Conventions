using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

/// <summary>
///     Delegate DistributedApplicationTestingConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
[PublicAPI]
public delegate void DistributedApplicationTestingConvention(IConventionContext context, IDistributedApplicationTestingBuilder builder);
