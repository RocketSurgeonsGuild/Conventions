using Microsoft.Extensions.Hosting;

namespace Clavus.Hosting;

/// <summary>
///     Extension members for IClavusContext configuration access.
/// </summary>
[PublicAPI]
public static class ClavusHostingContextExtensions
{
    extension(IClavusContext context)
    {
        /// <summary>
        ///     The underlying configuration. Populated from ConfigurationManager on web hosts.
        /// </summary>
        public IHostEnvironment Configuration =>
            context.Properties.Get<IHostEnvironment>()
            ?? throw new InvalidOperationException(
                "IHostEnvironment has not been registered in the Clavus context. Ensure the host populates context.Properties with IHostEnvironment."
            );
    }
}
