using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DependencyInjection;

/// <summary>
///     IServiceConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface IServiceConvention : IConvention
{
    /// <summary>
    ///     Register additional services with the service collection
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services);
}
