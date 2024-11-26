using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core;

namespace Sample.Core;

[ExportConvention]
[UnitTestConvention]
[AfterConvention(typeof(CoreConvention))]
public class TestConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<IService, TestService>();
    }
}
