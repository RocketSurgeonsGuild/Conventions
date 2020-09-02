using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Conventions.Internals
{
    internal interface IServiceFactoryAdapter
    {
        object CreateBuilder(IServiceCollection services, HostBuilderContext hostBuilderContext);

        IServiceProvider CreateServiceProvider(object containerBuilder);
    }
}