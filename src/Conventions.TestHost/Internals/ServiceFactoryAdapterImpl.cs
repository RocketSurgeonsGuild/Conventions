#if NETSTANDARD2_1
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Internals
{
    class ServiceFactoryAdapterImpl
    {
        private readonly IServiceFactoryAdapterProxy _impl;

        public ServiceFactoryAdapterImpl(IServiceFactoryAdapterProxy impl)
        {
            _impl = impl;
        }
        public object CreateBuilder(IServiceCollection services)
        {
            var result = _impl.CreateBuilder(services);
            Services = services;
            return result;
        }

        public IServiceCollection Services { get; set; }

        public IServiceProvider CreateServiceProvider(object containerBuilder)
        {
            return new ServiceCollection().BuildServiceProvider();
        }
    }
}
#endif