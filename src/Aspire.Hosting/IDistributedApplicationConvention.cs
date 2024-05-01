using Aspire.Hosting;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting;

/// <summary>
///     ILoggingConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IDistributedApplicationConvention : IConvention
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IDistributedApplicationBuilder builder);
}
