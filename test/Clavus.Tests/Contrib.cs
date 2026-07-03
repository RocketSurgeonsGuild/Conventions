using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Tests;

[ExportClavusPart]
internal sealed class Contrib : IServicePart
{
    public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services) { }
}
