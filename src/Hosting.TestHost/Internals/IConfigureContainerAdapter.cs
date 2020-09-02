using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Conventions.Internals
{
    internal interface IConfigureContainerAdapter
    {
        void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder);
    }
}