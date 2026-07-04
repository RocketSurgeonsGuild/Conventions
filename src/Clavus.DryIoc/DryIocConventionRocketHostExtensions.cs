using DryIoc;
// ReSharper disable once CheckNamespace

namespace Clavus.DryIoc;

/// <summary>
///     Class AutofacClavusHostHelpers.
/// </summary>
[PublicAPI]
public static class DryIocConventionHostExtensions
{
    /// <summary>
    ///     Uses Autofac.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ClavusContextBuilder UseDryIoc(this ClavusContextBuilder builder, IContainer? containerBuilder = null)
    {
        return builder.UseServiceProviderFactory<IContainer>(
            async (context, services, ct) =>
            {
                var c = containerBuilder ?? new Container();
                context.Set(services);
                await c.ApplyPartsAsync(context, ct);
                return new DryIocConventionServiceProviderFactory(context, c);
            }
        );
    }

}
