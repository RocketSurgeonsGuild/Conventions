using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.WebAssembly.Hosting;

/// <summary>
///     IWebAssemblyHostingConvention
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
public interface IWebAssemblyHostingConvention : IClavusPart
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IClavusContext context, WebAssemblyHostBuilder builder);
}
