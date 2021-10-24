using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core.Databases;

[assembly: Convention(typeof(DatabaseServiceConvention))]

namespace Sample.Core.Databases;

#region codeblock

public class DatabaseServiceConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var configurator = new DatabaseConfigurator();
        configurator.ApplyConventions(context);
    }
}

#endregion
