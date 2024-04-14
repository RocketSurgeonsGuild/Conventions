using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     IConfigurationAsyncConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IConfigurationAsyncConvention : IConvention
{
    /// <summary>
    ///     Register additional configuration providers with the configuration builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, IConfiguration configuration, IConfigurationBuilder builder, CancellationToken cancellationToken);
}