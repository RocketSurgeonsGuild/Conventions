using Microsoft.Extensions.DependencyInjection;

namespace Sample.Core;

[ClavusExport]
[UnitTestPart]
[AfterPart(typeof(CoreConvention))]
public class TestConvention : IServicePart
{
    public void Register(IClavusContext context, IServiceCollection services) => services.AddSingleton<IService, TestService>();
}
