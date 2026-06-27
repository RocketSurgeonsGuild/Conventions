using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.DependencyInjection;
namespace Sample.Core.Databases;

#region codeblock

[ExportConvention]
public class DatabaseServiceConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var configurator = new DatabaseConfigurator();
        configurator.ApplyConventions(context);
    }
}

#endregion
