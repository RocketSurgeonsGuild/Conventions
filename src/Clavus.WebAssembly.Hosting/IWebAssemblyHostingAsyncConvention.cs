using Clavus;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Clavus.WebAssembly.Hosting;

/// <summary>
///     IWebAssemblyHostingAsyncConvention
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
public interface IWebAssemblyHostingAsyncConvention : IClavusPart
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, WebAssemblyHostBuilder builder, CancellationToken cancellationToken);
}
