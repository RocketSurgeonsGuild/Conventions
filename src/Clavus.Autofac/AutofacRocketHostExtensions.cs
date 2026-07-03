using Autofac;
using Clavus.Autofac;

// ReSharper disable once CheckNamespace
namespace Clavus;

/// <summary>
///     Class AutofacRocketHostExtensions.
/// </summary>
[PublicAPI]
public static class AutofacConventionRocketHostExtensions
{
    /// <summary>
    ///     Uses Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder UseAutofac(this ClavusContextBuilder builder, ContainerBuilder? containerBuilder = null)
    {
        return builder.UseServiceProviderFactory<ContainerBuilder>(
            async (context, services, ct) =>
            {
                var c = containerBuilder ?? new ContainerBuilder();
                await c.ApplyPartsAsync(context, services, ct);
                return new AutofacConventionServiceProviderFactory(c);
            }
        );
    }

}
