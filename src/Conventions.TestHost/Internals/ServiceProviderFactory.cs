#if NETSTANDARD2_1
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Conventions.Internals
{
    internal static class ServiceProviderFactory
    {
        public static IHasServices ConfigureFactory(IHostBuilder hostBuilder, Type containerBuilder, IServiceFactoryAdapterProxy proxy)
        {
            return (IHasServices)typeof(ServiceProviderFactory)
               .GetMethod(nameof(ConfigureInternal), BindingFlags.Static | BindingFlags.NonPublic)
               .MakeGenericMethod(containerBuilder)
               .Invoke(null, new object[] { hostBuilder, proxy });
        }

        private static IHasServices ConfigureInternal<TContainerBuilder>(IHostBuilder hostBuilder, IServiceFactoryAdapterProxy proxy)
        {
            var factory = new ServiceProviderFactory<TContainerBuilder>(proxy);
            hostBuilder
               .ConfigureServices(
                    services => { hostBuilder.UseServiceProviderFactory(factory); }
                );

            return factory;
        }
    }

    interface IHasServices
    {
        IServiceCollection Services { get; }
    }

    internal class ServiceProviderFactory<TContainerBuilder> : IServiceProviderFactory<TContainerBuilder>, IHasServices
    {
        private readonly IServiceFactoryAdapterProxy _proxy;

        public ServiceProviderFactory(IServiceFactoryAdapterProxy proxy)
        {
            _proxy = proxy;
        }

        public TContainerBuilder CreateBuilder(IServiceCollection services)
        {
            Services = services;
            return (TContainerBuilder)_proxy.CreateBuilder(services);
        }

        public IServiceCollection Services { get; set; }

        public IServiceProvider CreateServiceProvider(TContainerBuilder containerBuilder)
        {
            return new ServiceCollection().BuildServiceProvider();
        }
    }
}
#endif