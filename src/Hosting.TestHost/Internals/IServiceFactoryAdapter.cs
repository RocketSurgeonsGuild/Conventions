using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Hosting.Internals;

internal interface IServiceFactoryAdapter
{
    object CreateBuilder(IServiceCollection services, HostBuilderContext hostBuilderContext);

    IServiceProvider CreateServiceProvider(object containerBuilder);
}
