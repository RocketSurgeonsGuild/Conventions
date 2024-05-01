using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

/// <summary>
///     ILoggingConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IDistributedApplicationTestingAsyncConvention : IConvention
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, IDistributedApplicationTestingBuilder builder, CancellationToken cancellationToken);
}
