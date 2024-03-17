#if NET8_0_OR_GREATER
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     ILoggingConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface IHostApplicationConvention : IConvention
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IHostApplicationBuilder builder);
}
#endif
