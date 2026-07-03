using Microsoft.Extensions.Configuration;

namespace Clavus;

/// <summary>
///     Extension members for IClavusContext configuration access.
/// </summary>
[PublicAPI]
public static class ClavusConfigurationContextExtensions
{
    extension(IClavusContext context)
    {
        /// <summary>
        ///     The underlying configuration. Populated from ConfigurationManager on web hosts.
        /// </summary>
        public IConfiguration Configuration =>
            context.Properties.Get<IConfiguration>()
            ?? throw new InvalidOperationException(
                "IConfiguration has not been registered in the Clavus context. Ensure the host populates context.Properties with IConfiguration."
            );
    }
}
