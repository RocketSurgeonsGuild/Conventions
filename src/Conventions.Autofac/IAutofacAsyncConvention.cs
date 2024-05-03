using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

/// <summary>
///     IAutofacConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IAutofacAsyncConvention : IConvention
{
    /// <summary>
    ///     Register additional things with the container
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="container"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(
        IConventionContext context,
        IConfiguration configuration,
        IServiceCollection services,
        ContainerBuilder container,
        CancellationToken cancellationToken
    );
}