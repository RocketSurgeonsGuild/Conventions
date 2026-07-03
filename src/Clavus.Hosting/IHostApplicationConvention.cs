using Microsoft.Extensions.Hosting;

namespace Clavus.Hosting;

/// <summary>
///     IHostApplicationPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IHostApplicationPart<in TBuilder> : IClavusPart
    where TBuilder : IHostApplicationBuilder
{
    /// <summary>
    ///     Register an event to happen when a host application is being configured
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IClavusContext context, TBuilder builder);
}
