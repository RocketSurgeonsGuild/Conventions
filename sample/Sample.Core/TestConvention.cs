using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus.DependencyInjection;

namespace Sample.Core;

[ExportClavusPart]
[UnitTestPart]
[AfterPart(typeof(CoreConvention))]
public class TestConvention : IServicePart
{
    public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services) => services.AddSingleton<IService, TestService>();
}
