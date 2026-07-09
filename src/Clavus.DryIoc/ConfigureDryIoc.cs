using DryIoc;

// ReSharper disable once CheckNamespace

namespace Clavus.DryIoc;

[ClavusExport]
internal class ConfigureDryIoc : ISetupPart
{
    public void Register(IClavusContext context)
    {
        context.UseServiceProviderFactory<IContainer>(
             async (context, services, ct) =>
            {
                var c = context.GetOrAdd<IContainer>(() => new Container());
                context.Set(services);
                await c.ApplyDryIoc(context, ct);
                return new DryIocConventionServiceProviderFactory(context, services, c);
            }
        );
    }
}
