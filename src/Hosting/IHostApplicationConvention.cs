using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     IHostApplicationConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IHostApplicationConvention<in TBuilder> : IConvention
    where TBuilder : IHostApplicationBuilder
{
    /// <summary>
    ///     Register an event to happen when a host application is being configured
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, TBuilder builder);
}