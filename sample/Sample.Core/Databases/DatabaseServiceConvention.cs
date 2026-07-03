using Microsoft.Extensions.DependencyInjection;
namespace Sample.Core.Databases;

#region codeblock

[ExportClavusPart]
public class DatabaseServicePart : IServiceAsyncPart
{
    public async ValueTask Register(IClavusContext context, IServiceCollection services, CancellationToken cancellationToken = default)
    {
        var configurator = new DatabaseConfigurator();
        await configurator.ApplyPartsAsync(context, cancellationToken: cancellationToken);
    }
}

#endregion
