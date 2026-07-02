using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Clavus.Autofac;

/// <summary>
///     IAutofacConvention
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IAutofacConvention : IClavusPart
{
    /// <summary>
    ///     Register additional things with the container
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="container"></param>
    void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services, ContainerBuilder container);
}
