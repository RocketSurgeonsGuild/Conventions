using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus.DependencyInjection;

namespace Rocket.Surgery.Clavus.Tests;

[ExportClavusPart]
internal sealed class Contrib : IServicePart
{
    public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services) { }
}
