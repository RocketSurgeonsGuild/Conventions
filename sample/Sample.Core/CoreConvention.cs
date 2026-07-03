using Microsoft.Extensions.DependencyInjection;

namespace Sample.Core;

[ExportClavusPart]
public class CoreConvention : IServicePart
{
    public void Register(IClavusContext context, IServiceCollection services) => services.AddSingleton<IService, AService>();
}
