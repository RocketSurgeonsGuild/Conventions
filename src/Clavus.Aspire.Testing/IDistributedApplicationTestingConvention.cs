using Aspire.Hosting.Testing;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.Aspire.Testing;

/// <summary>
///     ILoggingPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IDistributedApplicationTestingConvention : IClavusPart
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IClavusContext context, IDistributedApplicationTestingBuilder builder);
}
