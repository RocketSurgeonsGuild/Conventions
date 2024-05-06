using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     IHostApplicationAsyncConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IHostApplicationAsyncConvention<in TBuilder> : IConvention
    where TBuilder : IHostApplicationBuilder
{
    /// <summary>
    ///     Register an event to happen when a host application is being configured
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, TBuilder builder, CancellationToken cancellationToken);
}