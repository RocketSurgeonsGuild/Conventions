using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.DryIoc;

/// <summary>
///     IDryIocConvention
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IDryIocAsyncConvention : IClavusPart
{
    /// <summary>
    ///     Register additional things with the container
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="container"></param>
    /// <param name="cancellationToken"></param>
    ValueTask<IContainer> Register(
        IClavusContext context,
        IConfiguration configuration,
        IServiceCollection services,
        IContainer container,
        CancellationToken cancellationToken
    );
}
