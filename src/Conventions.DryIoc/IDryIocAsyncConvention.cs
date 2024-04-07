using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.DryIoc;

/// <summary>
///     IDryIocConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IDryIocAsyncConvention : IConvention
{
    /// <summary>
    ///     Register additional things with the container
    /// </summary>
    /// <param name="conventionContext"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="container"></param>
    /// <param name="cancellationToken"></param>
    IContainer Register(
        IConventionContext conventionContext,
        IConfiguration configuration,
        IServiceCollection services,
        IContainer container,
        CancellationToken cancellationToken
    );
}