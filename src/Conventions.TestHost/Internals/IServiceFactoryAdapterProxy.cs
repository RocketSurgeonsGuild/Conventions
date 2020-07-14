#if NETSTANDARD2_1
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Internals
{
    internal interface IServiceFactoryAdapterProxy
    {
        object CreateBuilder(IServiceCollection services);

        IServiceProvider CreateServiceProvider(object containerBuilder);
    }
}
#endif