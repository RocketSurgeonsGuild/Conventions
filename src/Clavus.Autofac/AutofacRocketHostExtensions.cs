using Autofac;
using Clavus.Autofac;

// ReSharper disable once CheckNamespace
namespace Clavus;

[ClavusExport]
internal class ConfigureAutofac : ISetupPart
{
    public void Register(IClavusContext context)
    {
        context.UseServiceProviderFactory<ContainerBuilder>(
             async (context, services, ct) =>
            {
                var c = context.GetOrAdd(() => new ContainerBuilder());
                context.Set(services);
                await c.ApplyAutofac(context, ct);
                return new AutofacConventionServiceProviderFactory(c);
            }
        );
    }
}
