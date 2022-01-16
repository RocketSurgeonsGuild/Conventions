using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     A convention service provider factory
/// </summary>
public interface IConventionServiceProviderFactory
{
    /// <summary>
    ///     Create the service provider from the services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    IServiceProvider CreateServiceProvider(IServiceCollection services, IConventionContext conventionContext);
}
