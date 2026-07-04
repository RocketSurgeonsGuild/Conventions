using Microsoft.Extensions.DependencyInjection;

namespace Sample.Core;

[ClavusExport]
public class CoreConvention : IServicePart
{
    public void Register(IClavusContext context, IServiceCollection services) => services.AddSingleton<IService, AService>();
}
