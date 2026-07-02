using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus.DependencyInjection;
namespace Sample.Core.Databases;

#region codeblock

[ExportClavusPart]
public class DatabaseServicePart : IServicePart
{
    public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services)
    {
        var configurator = new DatabaseConfigurator();
        configurator.ApplyConventions(context);
    }
}

#endregion
