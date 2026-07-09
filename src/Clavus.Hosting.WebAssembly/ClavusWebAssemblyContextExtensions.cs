using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Clavus.Hosting.WebAssembly;

/// <summary>
///     Extension members for IClavusContext configuration access.
/// </summary>
[PublicAPI]
public static class ClavusWebAssemblyContextExtensions
{
    extension(IClavusContext context)
    {
        /// <summary>
        ///     The underlying configuration. Populated from ConfigurationManager on web hosts.
        /// </summary>
        public IWebAssemblyHostEnvironment Configuration =>
            context.Properties.Get<IWebAssemblyHostEnvironment>()
            ?? throw new InvalidOperationException(
                "IWebAssemblyHostEnvironment has not been registered in the Clavus context. Ensure the host populates context.Properties with IWebAssemblyHostEnvironment."
            );
    }
}
