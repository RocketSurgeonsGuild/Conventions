using Autofac;
using Clavus.Autofac;

// ReSharper disable once CheckNamespace
namespace Clavus;

/// <summary>
///     Class AutofacClavusHostHelpers.
/// </summary>
[PublicAPI]
public static class AutofacConventionHostExtensions
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
                context.Set(services);
                await c.ApplyAutofac(context, ct);
                return new AutofacConventionServiceProviderFactory(c);
            }
        );
    }
}
