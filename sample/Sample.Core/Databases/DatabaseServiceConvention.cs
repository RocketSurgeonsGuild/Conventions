using Microsoft.Extensions.DependencyInjection;
namespace Sample.Core.Databases;

#region codeblock

[ClavusExport]
public class DatabaseServicePart : IServiceAsyncPart
{
    public async ValueTask Register(IClavusContext context, IServiceCollection services, CancellationToken cancellationToken = default)
    {
        var configurator = new DatabaseConfigurator();
        await configurator.ApplyDatabaseConfigurator(context, cancellationToken: cancellationToken);
    }
}

#endregion
