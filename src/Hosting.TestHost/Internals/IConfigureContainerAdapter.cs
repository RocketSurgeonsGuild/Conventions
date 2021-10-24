using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Hosting.Internals;

internal interface IConfigureContainerAdapter
{
    void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder);
}
