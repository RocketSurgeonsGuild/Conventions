using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DependencyInjection;

/// <summary>
///     IServiceAsyncConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IServiceAsyncConvention : IConvention
{
    /// <summary>
    ///     Register additional services with the service collection
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken);
}